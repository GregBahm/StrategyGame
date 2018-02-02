using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public static class UnitBattleApplication
{
    public const int MeleeAttackEnduranceCost = 10;
    public const int RangedAttackEnduranceCost = 10;

    public static IEnumerable<CombatLogItem> DoUnit(UnitState unit, BattlefieldState battlefield)
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
                IEnumerable<RangedAttack> rangedAttacks = unit.RangedAttacks.Where(attack => attack.Ammunition > 0);
                if(rangedAttacks.Any())
                {
                    foreach (RangedAttack rangedAttackttack in rangedAttacks)
                    {
                        IEnumerable<CombatLogItem> rangedAttackLogs = DoRangedAttack(unit, rangedAttackttack, battlefield);
                        ret.Concat(rangedAttackLogs);
                    }
                }
                else
                {
                    MoveUnit(unit, battlefield.Distances, battlefield.Collisions);
                }
            }
        }
        RecoverExhaustion(unit);

        HandleRegeneration(unit);
        
        HandleMoral(unit, battlefield);

        HandleDamageOverTime();

        return ret;
    }

    private static void MoveUnit(UnitState unit, BattlefieldDistances distances, BitArray collisions)
    {
        UnitLocation newLocation = distances.GetNextPosition(unit.Location, unit.Allegiance, collisions, distances);
        collisions[BattlefieldMover.UvToIndex(unit.Location)] = false;
        collisions[BattlefieldMover.UvToIndex(newLocation)] = true;
        unit.Location = newLocation;
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
        BattlefieldState battlefield)
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
        BattlefieldState battlefield)
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
        BattlefieldState battlefield)
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
        BattlefieldState battlefield)
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
        BattlefieldState battlefield)
    {
        int actualDamage = rangedAttack.AttackPower - target.Defense.Armor;
        ApplyDamageTo(target, actualDamage, battlefield);
        return new CombatLogItem(attacker, target, actualDamage);
    }

    private static void ApplyDamageTo(UnitState target, int damage, BattlefieldState battlefield)
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

    private static IEnumerable<UnitState> GetAdjacentUnits(UnitState unit, BattlefieldState battlefield)
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
        if (unit.Allegiance == UnitAllegiance.Berzerk)
        {
            return false;
        }
        return unit.Allegiance == possibleAlly.Allegiance;
    }

    public static bool GetIsEnemy(UnitState unit, UnitState possibleEnemy)
    {
        if(unit.Allegiance == UnitAllegiance.Berzerk)
        {
            return true;
        }
        return unit.Allegiance != possibleEnemy.Allegiance;
    }
    
    private static bool GetIsExhausted(UnitState unit)
    {
        return unit.Emotions.Endurance.Current <= 0;
    }

    private static void HandleMoral(UnitState unit, BattlefieldState battlefield)
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
