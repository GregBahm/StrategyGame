using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AdjacencyFinder
{
    private struct PositionOffset
    {
        private readonly int _xOffset;
        private readonly int _yOffset;

        public PositionOffset(int xOffset, int yOffset)
        {
            _xOffset = xOffset;
            _yOffset = yOffset;
        }

        internal UnitLocation Offset(int xPos, int yPos)
        {
            return new UnitLocation(xPos + _xOffset, yPos + _yOffset);
        }
    }
    
    private static Dictionary<int, IEnumerable<PositionOffset>> _positionsDictionary;

    static AdjacencyFinder()
    {
        _positionsDictionary = new Dictionary<int, IEnumerable<PositionOffset>>();
    }

    private static IEnumerable<PositionOffset> GetOffsetsForSize(int size)
    {
        if (_positionsDictionary.ContainsKey(size))
        {
            return _positionsDictionary[size];
        }
        List<PositionOffset> positions = new List<PositionOffset>();
        for (int i = -1; i < (size + 1); i++)
        {
            for (int j = -1; j < (size + 1); j++)
            {
                if(IsPerimiter(i, j, size))
                {
                    positions.Add(new PositionOffset(i, j));
                }
            }
        }
        _positionsDictionary.Add(size, positions);
        return positions;
    }

    private static bool IsPerimiter(int xDimension, int yDimension, int size)
    {
        return IsPerimiter(xDimension, size) || IsPerimiter(yDimension, size);
    }

    internal static IEnumerable<UnitLocation> GetRangedSearchPositions(UnitLocation location, int rangedAttackMid)
    {
        IEnumerable<UnitLocation> baseLocations = GetBaseRangedSearchPositions(location, rangedAttackMid);
        foreach (UnitLocation baseLocation in baseLocations)
        {
            yield return ClipToBounds(baseLocation);
        }
    }
    internal static IEnumerable<UnitLocation> GetBaseRangedSearchPositions(UnitLocation location, int rangedAttackMid)
    {
        yield return new UnitLocation(location.XPos + rangedAttackMid, location.YPos);
        yield return new UnitLocation(location.XPos - rangedAttackMid, location.YPos);
        yield return new UnitLocation(location.XPos, location.YPos + rangedAttackMid);
        yield return new UnitLocation(location.XPos, location.YPos - rangedAttackMid);
        int diagonal = (int)(rangedAttackMid * .7f);
        yield return new UnitLocation(location.XPos + diagonal, location.YPos + diagonal);
        yield return new UnitLocation(location.XPos + diagonal, location.YPos - diagonal);
        yield return new UnitLocation(location.XPos - diagonal, location.YPos - diagonal);
        yield return new UnitLocation(location.XPos - diagonal, location.YPos + diagonal);
    }

    private static bool IsPerimiter(int dimension, int size)
    {
        return dimension == -1 || dimension == size;
    }

    public static IEnumerable<UnitLocation> GetAdjacentPositions(UnitLocation location, int size)
    {
        return GetAdjacentPositions(location.XPos, location.YPos, size);
    }
    public static IEnumerable<UnitLocation> GetAdjacentPositions(UnitState unit)
    {
        return GetAdjacentPositions(unit.Location.XPos, unit.Location.YPos, unit.Size);
    }
    public static IEnumerable<UnitLocation> GetAdjacentPositions(int xPos, int yPos, int size)
    {
        IEnumerable<PositionOffset> offsets = GetOffsetsForSize(size);
        IEnumerable<UnitLocation> surroundingLocations = offsets.Select(item => item.Offset(xPos, yPos));
        IEnumerable<UnitLocation> withinBounds = surroundingLocations.Where(IsWithinBounds);
        return withinBounds;
    }

    public static UnitLocation ClipToBounds(UnitLocation location)
    {
        int newX = Mathf.Clamp(location.XPos, 0, BattlefieldMover.HorizontalResolution - 1 );
        int newY = Mathf.Clamp(location.YPos, 0, BattlefieldMover.VerticalResolution - 1);
        return new UnitLocation(newX, newY);
    }

    public static bool IsWithinBounds(UnitLocation location)
    {
        return location.XPos >= 0
            && location.YPos >= 0
            && location.XPos < BattlefieldMover.HorizontalResolution
            && location.YPos < BattlefieldMover.VerticalResolution;
    }
}