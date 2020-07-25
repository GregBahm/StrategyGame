
using System;
using System.Collections.Generic;
using System.Linq;

public class MeleeAttackEffector : BattalionEffector
{
    private readonly int weaponStrength;
    private readonly MeleeAttackType attackType;
    private readonly DamageType damageType;

    public MeleeAttackEffector(int weaponStrength, 
        MeleeAttackType attackType = MeleeAttackType.Regular,
        DamageType damageType = DamageType.Regular)
    {
        this.weaponStrength = weaponStrength;
        this.attackType = attackType;
        this.damageType = damageType;
    }

    public override BattalionBattleEffects GetEffect(BattalionState self, BattleStageSide allies, BattleStageSide enemies)
    {
        BattalionEffectsBuilder builder = new BattalionEffectsBuilder(this);
        if(self.Position.X == 0)
        {
            BattalionState target = enemies.GetTargetFor(self.Position);
            if (attackType == MeleeAttackType.Regular || attackType == MeleeAttackType.Charging)
            {
                DoMeleeAttack(builder, self, target);
            }
            if (attackType == MeleeAttackType.Charging)
            {
                IEnumerable<BattalionState> chargeTargets = GetChargeTargets(enemies, target);
                foreach (BattalionState chargeTarget in chargeTargets)
                {
                    DoMeleeAttack(builder, self, chargeTarget);
                }
            }
        }
        return builder.ToEffects();
    }

    /// <summary>
    /// Returns all front units until a unit has anticharge
    /// </summary>
    private IEnumerable<BattalionState> GetChargeTargets(BattleStageSide enemies, BattalionState regularTarget)
    {
        IEnumerable<BattalionState> frontLineUnits = enemies.Where(unit => unit.Position.IsFrontLine);
        if(frontLineUnits.Any(IsAntiCharge))
        {
            return new BattalionState[0];
        }
        else
        {
            return frontLineUnits.Where(unit => unit != regularTarget);
        }
    }

    private static bool IsAntiCharge(BattalionState state)
    {
        return state.GetAttribute(BattalionAttribute.AntiCharge) > 0;
    }

    private void DoMeleeAttack(BattalionEffectsBuilder builder, BattalionState self, BattalionState target)
    {
        int strength = self.GetAttribute(BattalionAttribute.Strength);
        BattalionAttribute damageAttribute = GetDamageAttributeFor(damageType);
        builder.Add(target.Id, damageAttribute, strength + weaponStrength);

        int retributionDamage = target.GetAttribute(BattalionAttribute.MeleeRetribution);
        if(retributionDamage > 0)
        {
            builder.Add(self.Id, BattalionAttribute.Damage, retributionDamage);
        }
    }
}
