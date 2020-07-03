
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
        if(allies.GetPosition(self) == BattlePosition.Front)
        {
            if(attackType == MeleeAttackType.Regular)
            {
                BattalionState target = enemies.GetFirstOfRank(BattlePosition.Front);
                DoMeleeAttack(builder, self, target);
            }
            if(attackType == MeleeAttackType.Charging)
            {
                IEnumerable<BattalionState> chargeTargets = GetChargeTargets(enemies);
                foreach (BattalionState target in chargeTargets)
                {
                    DoMeleeAttack(builder, self, target);
                }
            }
        }
        return builder.ToEffects();
    }

    /// <summary>
    /// Returns all front and mid units until a unit has anticharge
    /// </summary>
    private IEnumerable<BattalionState> GetChargeTargets(BattleStageSide enemies)
    {
        foreach (BattalionState unit in enemies.AllUnits)
        {
            bool antiCharge = unit.GetAttribute(BattalionAttribute.AntiCharge) > 0;
            if(antiCharge)
            {
                yield return unit;
                break;
            }
            BattlePosition pos = enemies.GetPosition(unit);
            if(pos == BattlePosition.Front || pos == BattlePosition.Mid)
            {
                yield return unit;
            }
        }
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
