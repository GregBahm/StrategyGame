using System.Collections.Generic;
using System.Linq;
using System;
public class BattleResolver
{
    private readonly List<UnitStateBuilder> _attackingUnits;
    private readonly List<UnitStateBuilder> _defendingUnits;
    private readonly Battlefield _battlefield;

    public BattleResolver(IEnumerable<UnitStateBuilder> attackingUnits,
        IEnumerable<UnitStateBuilder> defendingUnits,
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
        foreach (UnitStateBuilder unit in _attackingUnits)
        {
            ApplyUnitAction(unit, _attackingUnits, _defendingUnits);
        }
        foreach (UnitStateBuilder unit in _defendingUnits)
        {
            ApplyUnitAction(unit, _defendingUnits, _attackingUnits);
        }
        return GetBattleRound();
    }

    private void ApplyUnitAction(UnitStateBuilder unit, List<UnitStateBuilder> allies, List<UnitStateBuilder> enemies)
    {
        // TODO: Apply unit action
        throw new NotImplementedException();
    }

    private BattleRound GetBattleRound()
    {
        UnitState[] attackingState = _attackingUnits.Select(item => item.AsReadonly()).ToArray();
        UnitState[] defendingState = _defendingUnits.Select(item => item.AsReadonly()).ToArray();
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
