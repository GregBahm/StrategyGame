using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

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
        //_battlefield.UpdatePositions(_units); TODO: Sort this out
        List<CombatLogItem> logItems = new List<CombatLogItem>(); 
        foreach (UnitState unit in _units)
        {
            IEnumerable<CombatLogItem> unitCombatItems = UnitBattleApplication.DoUnit(unit, _battlefield);
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
    public const int MeleeAttackEnduranceCost = 10;
    public const int RangedAttackEnduranceCost = 10;

    public static IEnumerable<CombatLogItem> DoUnit(UnitState unit, Battlefield battlefield)
    {
        IEnumerable<CombatLogItem> ret = new CombatLogItem[0];
        if (unit.IsDefeated)
        {
            return ret;
        }

        CooldownCooldowns(unit);

        if(!unit.Emotions.IsRouting && !GetIsExhausted(unit))
        {
            IEnumerable<UnitState> adjacentEnemies = GetAdjacentUnits(unit, battlefield).Where(adjacentUnit => !unit.IsDefeated && GetIsEnemy(unit, adjacentUnit));
            if(adjacentEnemies.Any())
            {
                foreach (MeleeAttack attack in unit.MeleeAttacks.Where(attack => attack.Cooldown.Current < 1))
                {
                    IEnumerable<CombatLogItem> meleeAttackLogs = DoMeleeAttack(unit, attack, adjacentEnemies, battlefield);
                    ret.Concat(meleeAttackLogs);
                }
            }
            else
            {
                foreach (RangedAttack rangedAttackttack in unit.RangedAttacks.Where(attack => attack.Ammunition > 0))
                {
                    IEnumerable<CombatLogItem> rangedAttackLogs = DoRangedAttack(unit, rangedAttackttack, battlefield);
                    ret.Concat(rangedAttackLogs);
                }
            }
        }
        RecoverExhaustion(unit);

        HandleRegeneration(unit);
        
        HandleMoral(unit, battlefield);

        HandleDamageOverTime();

        return ret;
    }

    private static void CooldownCooldowns(UnitState unit)
    {
        foreach (MeleeAttack attack in unit.MeleeAttacks)
        {
            if (attack.Cooldown.Current > 0)
            {
                attack.Cooldown.Current--;
            }
        }
    }

    private static void HandleDamageOverTime()
    {
        // TODO: Put burning here
        // TODO: Put bleeding here
        // TODO: Put freezing here
        // TODO: Put poisons heres
    }

    private static IEnumerable<CombatLogItem> DoMeleeAttack(UnitState unit, 
        MeleeAttack attack, 
        IEnumerable<UnitState> adjacentEnemies, 
        Battlefield battlefield)
    {
        // Incure endurance cost
        unit.Emotions.Endurance.Current -= MeleeAttackEnduranceCost;

        // Reset cooldown
        attack.Cooldown.Current = attack.Cooldown.Max;

        // Figure out who to attack exactly
        IEnumerable<UnitState> allTargets = GetAllTargets(attack.AreaOfEffect, adjacentEnemies, battlefield);
        
        foreach (UnitState target in allTargets)
        {
            yield return ApplyMeleeAttackOn(target, unit, attack, battlefield);
        }
    }

    private static CombatLogItem ApplyMeleeAttackOn(UnitState target, 
        UnitState attacker, 
        MeleeAttack attack, 
        Battlefield battlefield)
    {
        // Do the damage
        int baseDamage = attack.AttackPower + attacker.Offense.Strength;
        int armor = target.Defense.Armor;

        int actualDamage = baseDamage - armor;
        ApplyDamageTo(target, actualDamage, battlefield);
        return new CombatLogItem(attacker, target, actualDamage);
    }

    private static IEnumerable<UnitState> GetAllTargets(AreaOfEffectType areaOfEffect, 
        IEnumerable<UnitState> adjacentEnemies, 
        Battlefield battlefield)
    {
        switch (areaOfEffect)
        {
            //TODO: Handle other area of effects
            case AreaOfEffectType.RingAroundUnit:
                return adjacentEnemies;
            case AreaOfEffectType.SingleTarget:
            default:
                return new UnitState[] { adjacentEnemies.First() };
        }
    }

    private static IEnumerable<CombatLogItem> DoRangedAttack(UnitState unit, 
        RangedAttack rangedAttack, 
        Battlefield battlefield)
    {
        // Incure endurance cost
        unit.Emotions.Endurance.Current -= RangedAttackEnduranceCost;

        rangedAttack.Ammunition -= 1;
        IEnumerable<UnitState> targets = battlefield.GetRangedTargetFor(unit, rangedAttack);
        foreach (UnitState target in targets)
        {
            yield return ApplyRangedAttackOn(target, unit, rangedAttack, battlefield);
        }
    }

    private static CombatLogItem ApplyRangedAttackOn(UnitState target, 
        UnitState attacker, 
        RangedAttack rangedAttack, 
        Battlefield battlefield)
    {
        int actualDamage = rangedAttack.AttackPower - target.Defense.Armor;
        ApplyDamageTo(target, actualDamage, battlefield);
        return new CombatLogItem(attacker, target, actualDamage);
    }

    private static void ApplyDamageTo(UnitState target, int damage, Battlefield battlefield)
    {
        target.HitPoints.Current -= damage;

        // If this kills the unit, eleminate them
        if (target.HitPoints.Current < 1)
        {
            target.IsDefeated = true;
            IEnumerable<UnitState> adjacentAllies = 
                GetAdjacentUnits(target, battlefield).Where(adjacentUnit => !adjacentUnit.IsDefeated && GetIsAlly(target, adjacentUnit));
            foreach (UnitState deathWitness in adjacentAllies)
            {
                deathWitness.Emotions.Moral.Current -= 1;
            }
        }

        // Otherwise, reduce the moral
        if (damage > 0)
        {
            target.Emotions.Moral.Current -= 1;
        }
    }

    private static IEnumerable<UnitState> GetAdjacentUnits(UnitState unit, Battlefield battlefield)
    {
        IEnumerable<UnitLocation> adjacentLocations = AdjacencyFinder.GetAdjacentPositions(unit);
        foreach (UnitLocation pos in adjacentLocations)
        {
            UnitState unitAtPos = battlefield.GetUnitAt(pos);
            if(unitAtPos != null)
            {
                yield return unitAtPos;
            }
        }
    }

    public static bool GetIsAlly(UnitState unit, UnitState possibleAlly)
    {
        if (unit.Allegiance == UnitAllegiance.AttacksAll)
        {
            return false;
        }
        return unit.Allegiance == possibleAlly.Allegiance;
    }

    public static bool GetIsEnemy(UnitState unit, UnitState possibleEnemy)
    {
        if(unit.Allegiance == UnitAllegiance.AttacksAll)
        {
            return true;
        }
        return unit.Allegiance != possibleEnemy.Allegiance;
    }
    
    private static bool GetIsExhausted(UnitState unit)
    {
        return unit.Emotions.Endurance.Current <= 0;
    }

    private static void HandleMoral(UnitState unit, Battlefield battlefield)
    {
        if(unit.Emotions.Moral.Current <= 0)
        {
            unit.Emotions.IsRouting = true;
            IEnumerable<UnitState> adjacentAllies =
                GetAdjacentUnits(unit, battlefield).Where(adjacentUnit => !adjacentUnit.IsDefeated && GetIsAlly(unit, adjacentUnit));
            foreach (UnitState adjacentAlly in adjacentAllies)
            {
                unit.Emotions.Moral.Current -= 1;
            }
        }
    }

    private static void HandleRegeneration(UnitState unit)
    {
        unit.HitPoints.Current = Mathf.Max(unit.HitPoints.Current + unit.Defense.Regeneration, unit.HitPoints.Max);
    }

    private static void RecoverExhaustion(UnitState unit)
    {
        unit.Emotions.Endurance.Current = Mathf.Max(unit.Emotions.Endurance.Current + 1, unit.Emotions.Endurance.Max);
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
