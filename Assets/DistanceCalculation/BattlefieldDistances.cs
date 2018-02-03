using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlefieldDistances
{
    public readonly BattlefieldDistance[] Distances;

    public BattlefieldDistances(BattlefieldDistance[] distances)
    {
        Distances = distances;
    }
    private BattlefieldDistance GetDistanceAt(UnitLocation location)
    {
        int index = BattlefieldMover.UvToIndex(location.XPos, location.YPos);
        return Distances[index];
    }
    public UnitLocation GetNextPosition(UnitLocation current, UnitAllegiance alligence, BitArray collisionBitarray, bool isRouting)
    {
        // TODO: Handle fleeing
        IEnumerable<UnitLocation> adjacent = AdjacencyFinder.GetAdjacentPositions(current.XPos, current.YPos, 1);
        IEnumerable<UnitLocation> notBlocked = adjacent.Where(item => !PositionOccupied(item, collisionBitarray));
        IEnumerable<DistanceCheck> distanceChecks = notBlocked.Select(item => new DistanceCheck(GetDistanceToEnemy(item, alligence), item));
        if(!distanceChecks.Any())
        {
            return current;
        }
        int bestDistance = distanceChecks.Min(item => item.Distance);
        IEnumerable<DistanceCheck> bestDistances = distanceChecks.Where(item => item.Distance == bestDistance);

        // TODO: Prioritize movement away from friends
        return bestDistances.First().Location;
    }

    public UnitLocation GetEnemyClosestTo(UnitLocation searchPoint, UnitAllegiance allegiance)
    {
        int dist = GetDistanceToEnemy(searchPoint, allegiance);
        if (dist == 0)
        {
            return searchPoint;
        }
        return GetEnemyClosestRecursive(searchPoint, allegiance);
    }

    private UnitLocation GetEnemyClosestRecursive(UnitLocation searchPoint, UnitAllegiance allegiance)
    {
        IEnumerable<UnitLocation> adjacentLocations = AdjacencyFinder.GetAdjacentPositions(searchPoint, 1);
        int closestDist = int.MaxValue;
        UnitLocation location;
        foreach (UnitLocation adjacentLocation in adjacentLocations)
        {
            int adjacentDist = GetDistanceToEnemy(searchPoint, allegiance);
            if (adjacentDist == 0)
            {
                return adjacentLocation;
            }
            else if (adjacentDist < closestDist)
            {
                closestDist = adjacentDist;
                location = adjacentLocation;
            }
        }
        return GetEnemyClosestRecursive(searchPoint, allegiance);
    }

    public int GetDistanceToEnemy(UnitLocation item,  UnitAllegiance alligence)
    {
        BattlefieldDistance dist = GetDistanceAt(item);
        int amount;
        switch (alligence)
        {
            case UnitAllegiance.Attacker:
                amount = Mathf.Min(dist.DefenderDistance, dist.NeutralDistance, dist.BerzerkerDistance);
                break;
            case UnitAllegiance.Defender:
                amount = Mathf.Min(dist.AttackerDistance, dist.NeutralDistance, dist.BerzerkerDistance);
                break;
            case UnitAllegiance.Neutral:
                amount = Mathf.Min(dist.AttackerDistance, dist.DefenderDistance, dist.BerzerkerDistance);
                break;
            case UnitAllegiance.Berzerk:
            default:
                amount = Mathf.Min(dist.AttackerDistance, dist.DefenderDistance, dist.NeutralDistance, dist.BerzerkerDistance);
                break;
        }
        return amount;
    }

    private static bool PositionOccupied(UnitLocation position, BitArray collisionBitarray)
    {
        int index = BattlefieldMover.UvToIndex(position);
        return collisionBitarray[index];
    }

    private struct DistanceCheck
    {
        public int Distance;
        public UnitLocation Location;

        public DistanceCheck(int distance, UnitLocation location)
        {
            Distance = distance;
            Location = location;
        }
    }
}
