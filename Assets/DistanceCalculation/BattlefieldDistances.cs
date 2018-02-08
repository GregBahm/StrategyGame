using System;
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

    private int GetBestDistance(IEnumerable<DistanceCheck> distanceChecks, bool isRouting)
    {
        if(isRouting)
        {
            return distanceChecks.Max(item => item.Distance);
        }
        return distanceChecks.Min(item => item.Distance);
    }

    public UnitLocation GetEnemyClosestTo(UnitLocation searchPoint, UnitAllegiance allegiance)
    {
        int dist = GetDistanceToEnemy(searchPoint, allegiance);
        if (dist == 1)
        {
            return searchPoint;
        }
        return GetEnemyClosestRecursive(searchPoint, allegiance);
    }

    private UnitLocation GetEnemyClosestRecursive(UnitLocation searchPoint, UnitAllegiance allegiance)
    {
        IEnumerable<UnitLocation> adjacentLocations = AdjacencyFinder.GetAdjacentPositions(searchPoint, 1);
        int closestDist = int.MaxValue;
        UnitLocation location = searchPoint;
        foreach (UnitLocation adjacentLocation in adjacentLocations)
        {
            int adjacentDist = GetDistanceToEnemy(adjacentLocation, allegiance);
            if (adjacentDist == 1)
            {
                return adjacentLocation;
            }
            else if (adjacentDist < closestDist)
            {
                closestDist = adjacentDist;
                location = adjacentLocation;
            }
        }
        if(location == searchPoint)
        {
            throw new System.Exception("GetEnemyClosestRecursive is broken");
        }
        return GetEnemyClosestRecursive(location, allegiance);
    }

    public int GetDistanceToEnemy(UnitLocation item,  UnitAllegiance alligence)
    {
        BattlefieldDistance dist = GetDistanceAt(item);
        switch (alligence)
        {
            case UnitAllegiance.Attacker:
                return Mathf.Min(dist.DefenderDistance, dist.NeutralDistance, dist.BerzerkerDistance);
            case UnitAllegiance.Defender:
                return Mathf.Min(dist.AttackerDistance, dist.NeutralDistance, dist.BerzerkerDistance);
            case UnitAllegiance.Neutral:
                return Mathf.Min(dist.AttackerDistance, dist.DefenderDistance, dist.BerzerkerDistance);
            case UnitAllegiance.Berzerk:
            default:
                return Mathf.Min(dist.AttackerDistance, dist.DefenderDistance, dist.NeutralDistance, dist.BerzerkerDistance);
        }
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
