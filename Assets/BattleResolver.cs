using System.Collections.Generic;
using System.Linq;
using System;
public class BattleResolver
{
    private readonly List<UnitState> _units;
    private readonly Battlefield _battlefield;
    private const int BattleRoundLimit = 2000;

    public BattleResolver(IEnumerable<UnitState> units,
        Battlefield battlefield)
    {
        _units = units.ToList();
        _battlefield = battlefield;
    }

    internal List<BattleRound> ResolveBattle()
    {
        List<BattleRound> ret = new List<BattleRound>();
        BattleStatus currentStatus = BattleStatus.Ongoing;
        for (int i = 0; i < BattleRoundLimit; i++)
        {
            if(currentStatus == BattleStatus.Ongoing)
            {
                BattleRound latestRound = AdvanceBattle();
                ret.Add(latestRound);
                currentStatus = latestRound.Status;
            }
            else
            {
                break;
            }
        }
        if(currentStatus == BattleStatus.Ongoing)
        {
            currentStatus = BattleStatus.Stalemate;
        }

        return ret;
    }

    public BattleRound AdvanceBattle()
    {
        _battlefield.UpdatePositions(_units);
        foreach (UnitState unit in _units)
        {
            UnitBattleApplication.DoUnit(unit, _battlefield);
        }
        return GetBattleRound();
    }

    private BattleRound GetBattleRound()
    {
        UnitStateRecord[] unitsRecord = _units.Select(item => item.AsReadonly()).ToArray();
        BattleStatus status = GetBattleStatus();
        return new BattleRound(unitsRecord, status);
    }

    private BattleStatus GetBattleStatus()
    {
        bool attackersAlive = false;
        bool defendersAlive = false;
        foreach (UnitState unit in _units.Where(unit => !unit.IsDefeated))
        {
            if(unit.Allegiance == UnitAllegiance.Attackers)
            {
                attackersAlive = true;
            }
            if(unit.Allegiance == UnitAllegiance.Defenders)
            {
                defendersAlive = true;
            }
            if (attackersAlive && defendersAlive)
            {
                return BattleStatus.Ongoing;
            }
        }
        if(!attackersAlive && !defendersAlive)
        {
            return BattleStatus.NoSurvivors;
        }
        return attackersAlive ? BattleStatus.AttackersVictorious : BattleStatus.DefendersVictorious;
    }
}

public static class UnitBattleApplication
{

    public static void DoUnit(UnitState unit, Battlefield battlefield)
    {
        if(unit.IsDefeated)
        {
            return;
        }
        
        if(!unit.Emotions.IsRouting && !GetIsExhausted(unit))
        {
            IEnumerable<UnitState> adjacentEnemies = GetAdjacentEnemies(unit, battlefield);
            if(adjacentEnemies.Any())
            {
                foreach (MeleeAttack attack in unit.MeleeAttacks)
                {
                    DoMeleeAttack(unit, attack, adjacentEnemies, battlefield);
                }
            }
            else
            {
                foreach (RangedAttack rangedAttackttack in unit.RangedAttacks.Where(attack => attack.Ammunition > 0))
                {
                    DoRangedAttack(unit, rangedAttackttack, battlefield);
                }
            }
        }
        RecoverExhaustion(unit);
        Regenerate(unit);
        HandleMoral(unit);
        HandleDamageOverTime();
    }

    private static void HandleDamageOverTime()
    {
        throw new NotImplementedException();
    }

    private static void DoMeleeAttack(UnitState unit, MeleeAttack attack, IEnumerable<UnitState> adjacentEnemies, Battlefield battlefield)
    {
        throw new NotImplementedException();
    }

    private static void DoRangedAttack(UnitState unit, RangedAttack rangedAttackttack, Battlefield battlefield)
    {
        throw new NotImplementedException();
    }

    private static IEnumerable<UnitState> GetAdjacentEnemies(UnitState unit, Battlefield battlefield)
    {
        IEnumerable<UnitLocation> adjacentLocations = AdjacencyFinder.GetAdjacentPositions(unit);
        foreach (UnitLocation pos in adjacentLocations)
        {
            UnitState unitAtPos = battlefield.GetUnitAt(pos);
            if(unitAtPos != null && GetIsEnemy(unit, unitAtPos))
            {
                yield return unitAtPos;
            }
        }
    }

    private static bool GetIsEnemy(UnitState unit, UnitState unitAtPos)
    {
        if(unit.Allegiance == UnitAllegiance.AttacksAll)
        {
            return true;
        }
        return unit.Allegiance != unitAtPos.Allegiance;
    }
    
    private static bool GetIsExhausted(UnitState unit)
    {
        return unit.Emotions.Endurance.Current <= 0;
    }

    private static void HandleMoral(UnitState unit)
    {
        throw new NotImplementedException();
    }

    private static void Regenerate(UnitState unit)
    {
        throw new NotImplementedException();
    }

    private static void RecoverExhaustion(UnitState unit)
    {
        throw new NotImplementedException();
    }
}

class AdjacencyFinder
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

        internal UnitLocation Offset(UnitPosition position)
        {
            return new UnitLocation(position.XPos + _xOffset, position.YPos + _yOffset);
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

    public static IEnumerable<UnitLocation> GetAdjacentPositions(UnitState unit)
    {
        IEnumerable<PositionOffset> offsets = GetOffsetsForSize(unit.Size);
        return offsets.Select(item => item.Offset(unit.Position));
    }
}