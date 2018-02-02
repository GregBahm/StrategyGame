using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattlefieldState
{
    private readonly UnitState[,] _unitsGrid;

    private readonly BitArray _collisions; 
    public BitArray Collisions { get{ return _collisions; } }

    private readonly BattlefieldDistances _distances;
    public BattlefieldDistances Distances { get{ return _distances; } }

    public BattlefieldState(List<UnitState> allUnits, BattlefieldMover mover)
    {
        BattlefieldStateForMover stateForMover = new BattlefieldStateForMover(allUnits);
        _distances = mover.GetDistances(stateForMover);
        _unitsGrid = CreateUnitsGrid(allUnits);
        _collisions = GetCollisionBitarray(allUnits);
    }

    private static UnitState[,] CreateUnitsGrid(List<UnitState> allUnits)
    {
        UnitState[,] ret = new UnitState[BattlefieldMover.HorizontalResolution, BattlefieldMover.VerticalResolution];
        foreach (UnitState unit in allUnits)
        {
            ret[unit.Location.XPos, unit.Location.YPos] = unit;
        }
        return ret;
    }

    internal UnitState GetUnitAt(UnitLocation pos)
    {
        return _unitsGrid[pos.XPos, pos.YPos];
    }

    internal IEnumerable<UnitState> GetRangedTargetFor(UnitState unit, RangedAttack rangedAttack)
    {
        // TODO: Implement Get Ranged Targets For
        throw new NotImplementedException();
    }

    private BitArray GetCollisionBitarray(IEnumerable<UnitState> allUnits)
    {
        BitArray ret = new BitArray(BattlefieldMover.BattlefieldResolution);
        foreach (UnitLocation location in allUnits.Select(item => item.Location))
        {
            int index = BattlefieldMover.UvToIndex(location.XPos, location.YPos);
            ret[index] = true;
        }
        return ret;
    }

    private UnitLocation[] GetMovedUnits(UnitLocation[] positions, UnitAllegiance allegiance, BitArray collisionBitarray, BattlefieldDistances distances)
    {
        List<UnitLocation> newLocations = new List<UnitLocation>();
        foreach (UnitLocation location in positions)
        {
            UnitLocation newLocation = distances.GetNextPosition(location, allegiance, collisionBitarray, distances);
            collisionBitarray[BattlefieldMover.UvToIndex(location)] = false;
            collisionBitarray[BattlefieldMover.UvToIndex(newLocation)] = true;
            newLocations.Add(newLocation);
        }
        return newLocations.ToArray();
    }
}
