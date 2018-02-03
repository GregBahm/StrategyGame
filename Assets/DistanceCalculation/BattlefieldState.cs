using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattlefieldState
{
    private readonly UnitState[,] _unitsGrid;
    private readonly BitArray _collisions; 
    private readonly BattlefieldDistances _distances;

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
        UnitState baseTarget = GetBaseRangedTargetFor(unit, rangedAttack);
        if(baseTarget != null)
        {
            //TODO: Handle getting splash targets for area of effect ranged attacks
            yield return baseTarget;
        }
    }

    private UnitState GetBaseRangedTargetFor(UnitState unit, RangedAttack rangedAttack)
    {
        int rangedAttackMid = (rangedAttack.MaximumRange + rangedAttack.MinimumRange) / 2;
        int requiredDist = (rangedAttack.MaximumRange - rangedAttack.MinimumRange) / 2;
        IEnumerable<UnitLocation> searchStartLocations = AdjacencyFinder.GetRangedSearchPositions(unit.Location, rangedAttackMid);
        foreach (UnitLocation searchStart in searchStartLocations)
        {
            int dist = _distances.GetDistanceToEnemy(searchStart, unit.Allegiance);
            if (dist < requiredDist)
            {
                UnitLocation location = _distances.GetEnemyClosestTo(searchStart, unit.Allegiance);
                return GetUnitAt(location);
            }
        }
        return null;
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

    public void MoveUnit(UnitState unit)
    {
        UnitLocation newLocation = _distances.GetNextPosition(unit.Location, unit.Allegiance, _collisions, unit.Emotions.IsRouting);
        _collisions[BattlefieldMover.UvToIndex(unit.Location)] = false;
        _collisions[BattlefieldMover.UvToIndex(newLocation)] = true;
        unit.Location = newLocation;
    }
}
