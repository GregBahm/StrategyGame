using System.Collections.Generic;
using System.Linq;
using System;

public class BattleResolver
{
    private readonly List<UnitState> _units;
    private readonly BattlefieldMover _mover;
    private const int BattleRoundLimit = 2000;

    public BattleResolver(IEnumerable<UnitState> units,
        BattlefieldMover battlefield)
    {
        _units = units.ToList();
        _mover = battlefield;
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
        BattlefieldState state = new BattlefieldState(_units, _mover);

        List<CombatLogItem> logItems = new List<CombatLogItem>();
        foreach (UnitState unit in _units)
        {
            IEnumerable<CombatLogItem> unitCombatItems = UnitBattleApplication.DoUnit(unit, state);
            logItems.AddRange(unitCombatItems);
        }

        return GetBattleRound(logItems);
    }

    private BattleRound GetBattleRound(List<CombatLogItem> logItems)
    {
        UnitStateRecord[] unitsRecord = _units.Select(item => item.AsReadonly()).ToArray();
        BattleStatus status = GetBattleStatus();
        return new BattleRound(unitsRecord, status, logItems);
    }

    private BattleStatus GetBattleStatus()
    {
        bool attackersAlive = false;
        bool defendersAlive = false;
        foreach (UnitState unit in _units.Where(unit => !unit.IsDefeated))
        {
            if(unit.Allegiance == UnitAllegiance.Attacker)
            {
                attackersAlive = true;
            }
            if(unit.Allegiance == UnitAllegiance.Defender)
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

public struct CombatLogItem
{
    private readonly UnitState _attacker;
    public UnitState Attacker{ get{ return _attacker; } }

    private readonly UnitState _attackee;
    public UnitState Attackee { get{ return _attackee; } }

    private readonly int _damage;
    public int Damage { get{ return _damage; } }

    public CombatLogItem(UnitState attacker, UnitState attackee, int damage)
    {
        _attacker = attacker;
        _attackee = attackee;
        _damage = damage;
    }
}
