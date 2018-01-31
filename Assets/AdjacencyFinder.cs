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
        internal UnitLocation Offset(UnitPosition position)
        {
            return Offset(position.XPos, position.YPos);
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
        return GetAdjacentPositions(unit.Position.XPos, unit.Position.YPos, unit.Size);
    }
    public static IEnumerable<UnitLocation> GetAdjacentPositions(int xPos, int yPos, int size)
    {
        IEnumerable<PositionOffset> offsets = GetOffsetsForSize(size);
        IEnumerable<UnitLocation> surroundingLocations = offsets.Select(item => item.Offset(xPos, yPos));
        IEnumerable<UnitLocation> withinBounds = surroundingLocations.Where(Battlefield.IsWithinBounds);
        return withinBounds;
    }
}