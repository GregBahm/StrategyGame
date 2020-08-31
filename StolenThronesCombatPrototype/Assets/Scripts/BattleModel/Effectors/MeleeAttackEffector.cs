
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

    public override IEnumerable<BattalionStateModifier> GetEffect(BattalionState self, BattleStateSide allies, BattleStateSide enemies)
    {
        BattalionEffectsBuilder builder = new BattalionEffectsBuilder(this);
        if(allies.GetPosition(self.Id) == 0)
        {
            if(attackType == MeleeAttackType.Regular)
            {
                BattleRank target = enemies.First();
                //DoMeleeAttack(builder, self, target);
            }
            if(attackType == MeleeAttackType.Charging)
            {
                //TODO: Implement this
            }
        }
        return builder.ToEffects();
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
