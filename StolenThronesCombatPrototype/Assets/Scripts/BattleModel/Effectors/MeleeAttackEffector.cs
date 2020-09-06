
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

    public override IEnumerable<BattalionStateModifier> GetEffect(BattalionBattleState self, BattleStateSide allies, BattleStateSide enemies)
    {
        BattalionEffectsBuilder builder = new BattalionEffectsBuilder(this);

        bool canAttack = GetCanAttack(self);
        if (self.Position == 0)
        {
            if(attackType == MeleeAttackType.Regular)
            {
                IEnumerable<BattalionBattleState> targets = enemies.Ranks[0];
                foreach (BattalionBattleState target in targets)
                {
                    DoMeleeAttack(builder, self, target);
                }
            }
        }
        return builder.ToEffects();
    }

    private bool GetCanAttack(BattalionBattleState self)
    {
        switch (attackType)
        {
            case MeleeAttackType.Regular:
            default:
                return self.Position == 0;
        }
    }

    private void DoMeleeAttack(BattalionEffectsBuilder builder, BattalionBattleState self, BattalionBattleState target)
    {
        int strength = self.GetAttribute(BattalionAttribute.Strength);
        int baseDamage = strength + weaponStrength;
        int fullDamage = baseDamage * self.RemainingUnits;
        BattalionAttribute damageAttribute = GetDamageAttributeFor(damageType);
        int damage = (int)(fullDamage * target.PresenceWithinRank);
        builder.Add(target.Id, damageAttribute, damage);

        int retributionDamage = target.GetAttribute(BattalionAttribute.MeleeRetribution);
        if (retributionDamage > 0)
        {
            int damageBack = (int)(retributionDamage * target.Presence);
            builder.Add(self.Id, BattalionAttribute.Damage, retributionDamage);
        }
    }
}
