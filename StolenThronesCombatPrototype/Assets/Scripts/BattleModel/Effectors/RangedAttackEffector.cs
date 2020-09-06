using System;
using System.Collections.Generic;
using System.Linq;

public class RangedAttackEffector : BattalionEffector
{
    private readonly int range;
    private readonly int weaponStrength;
    private readonly DamageType damageType;

    public RangedAttackEffector(int weaponStrength, 
        int range,
        DamageType damageType = DamageType.Regular)
    {
        this.weaponStrength = weaponStrength;
        this.range = range;
        this.damageType = damageType;
    }
    public override IEnumerable<BattalionStateModifier> GetEffect(BattalionBattleState self, BattleStateSide allies, BattleStateSide enemies)
    {
        BattalionEffectsBuilder builder = new BattalionEffectsBuilder(this);

        int reloadingState = self.GetAttribute(BattalionAttribute.ReloadingState);
        if(reloadingState > 0)
        {
            builder.Add(self.Id, BattalionAttribute.ReloadingState, -1);
        }
        else
        {
            if (self.Position <= range)
            {
                DoAttack(builder, self, enemies);
            }
        }
        return builder.ToEffects();
    }

    private void DoAttack(BattalionEffectsBuilder builder, BattalionBattleState self, BattleStateSide enemies)
    {
        BattalionAttribute damageAttribute = GetDamageAttributeFor(damageType);
        int totalDamage = weaponStrength * self.RemainingUnits;
        foreach (BattalionBattleState target in enemies.Ranks[0])
        {
            int damage = (int)(totalDamage * target.PresenceWithinRank);
            builder.Add(target.Id, damageAttribute, weaponStrength);
        }
        int reloadingSpeed = self.GetAttribute(BattalionAttribute.ReloadingSpeed);
        builder.Add(self.Id, BattalionAttribute.ReloadingState, reloadingSpeed);
    }
}