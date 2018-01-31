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
    public BattlefieldDistance GetDistanceAt(int x, int y)
    {
        int index = Battlefield.UvToIndex(x, y);
        return Distances[index];
    }
    public UnitLocation GetNextPosition(UnitLocation current, UnitAllegiance alligence, BitArray collisionBitarray, BattlefieldDistances distances)
    {
        IEnumerable<UnitLocation> adjacent = AdjacencyFinder.GetAdjacentPositions(current.XPos, current.YPos, 1);
        IEnumerable<UnitLocation> notBlocked = adjacent.Where(item => !PositionOccupied(item, collisionBitarray));
        IEnumerable<DistanceCheck> distanceChecks = notBlocked.Select(item => GetDistanceCheck(item, distances, alligence));
        if(!distanceChecks.Any())
        {
            return current;
        }
        int bestDistance = distanceChecks.Min(item => item.Distance);
        IEnumerable<DistanceCheck> bestDistances = distanceChecks.Where(item => item.Distance == bestDistance);

        // TODO: Prioritize movement away from friends
        return bestDistances.First().Location;
    }

    private DistanceCheck GetDistanceCheck(UnitLocation item, BattlefieldDistances distances,  UnitAllegiance alligence)
    {
        BattlefieldDistance dist = distances.GetDistanceAt(item.XPos, item.YPos);
        int amount;
        switch (alligence)
        {
            case UnitAllegiance.Attackers:
                amount = dist.EnemyDistance;
                break;
            case UnitAllegiance.Defenders:
                amount = dist.AlliedDistance;
                break;
            case UnitAllegiance.Neutrals:
                amount = dist.NeutralDistance;
                break;
            case UnitAllegiance.AttacksAll:
            default:
                amount = dist.BerzerkerDistance;
                break;
        }
        return new DistanceCheck(amount, item);
    }

    private static bool PositionOccupied(UnitLocation position, BitArray collisionBitarray)
    {
        int index = Battlefield.UvToIndex(position);
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
