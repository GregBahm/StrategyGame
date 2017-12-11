using System.Collections.Generic;
using System.Linq;
using System;
public class BattleResolver
{
    private readonly List<UnitState> _attackingUnits;
    private readonly List<UnitState> _defendingUnits;
    private readonly Battlefield _battlefield;

    public BattleResolver(IEnumerable<UnitState> attackingUnits,
        IEnumerable<UnitState> defendingUnits,
        Battlefield battlefield)
    {
        _attackingUnits = attackingUnits.ToList();
        _defendingUnits = defendingUnits.ToList();
        _battlefield = battlefield;
    }

    internal List<BattleRound> ResolveBattle()
    {
        List<BattleRound> ret = new List<BattleRound>();
        BattleStatus currentStatus = BattleStatus.Ongoing;
        while(currentStatus == BattleStatus.Ongoing)
        {
            BattleRound latestRound = AdvanceBattle();
            ret.Add(latestRound);
            currentStatus = latestRound.Status;
        }

        return ret;
    }

    public BattleRound AdvanceBattle()
    {
        _battlefield.UpdatePositions(_attackingUnits, _defendingUnits);
        foreach (UnitState unit in _attackingUnits)
        {
            ApplyUnitAction(unit, _attackingUnits, _defendingUnits);
        }
        foreach (UnitState unit in _defendingUnits)
        {
            ApplyUnitAction(unit, _defendingUnits, _attackingUnits);
        }
        return GetBattleRound();
    }

    private void ApplyUnitAction(UnitState unit, List<UnitState> allies, List<UnitState> enemies)
    {
        // TODO: Apply Unit Action

    }

    private BattleRound GetBattleRound()
    {
        UnitStateRecord[] attackingState = _attackingUnits.Select(item => item.AsReadonly()).ToArray();
        UnitStateRecord[] defendingState = _defendingUnits.Select(item => item.AsReadonly()).ToArray();
        BattleStatus status = GetBattleStatus();
        return new BattleRound(attackingState, defendingState, status);
    }

    private BattleStatus GetBattleStatus()
    {
        if (_attackingUnits.All(unit => unit.IsDefeated))
        {
            if (_defendingUnits.All(unit => unit.IsDefeated))
            {
                return BattleStatus.Draw;
            }
            return BattleStatus.DefendersVictorious;
        }
        if (_defendingUnits.All(unit => unit.IsDefeated))
        {
            return BattleStatus.AttackersVictorious;
        }
        return BattleStatus.Ongoing;
    }
}
