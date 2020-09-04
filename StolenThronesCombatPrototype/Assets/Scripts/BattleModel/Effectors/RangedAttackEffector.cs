using System;
using System.Collections.Generic;
using System.Linq;

public class RangedAttackEffector : BattalionEffector
{
    private readonly int range;
    private readonly int weaponStrength;
    private readonly int splashDamage;
    private readonly DamageType damageType;

    public RangedAttackEffector(int weaponStrength, 
        int range,
        DamageType damageType = DamageType.Regular,
        int splashDamage = 0)
    {
        this.weaponStrength = weaponStrength;
        this.range = range;
        this.damageType = damageType;
        this.splashDamage = splashDamage;
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
        throw new NotImplementedException(); //
        //builder.Add(target.Id, damageAttribute, weaponStrength);

        //if(splashDamage > 0)
        //{
        //    IEnumerable<BattalionState> splashTargets = GetSplashTargets(target, enemies);
        //    foreach (var item in splashTargets)
        //    {
        //        builder.Add(target.Id, damageAttribute, splashDamage);
        //    }
        //}

        //int reloadingSpeed = self.GetAttribute(BattalionAttribute.ReloadingSpeed);
        //builder.Add(self.Id, BattalionAttribute.ReloadingState, reloadingSpeed);
    }

    private IEnumerable<BattalionState> GetSplashTargets(BattalionState target, BattleStateSide enemies)
    {
        return new BattalionState[0];// TODO: This
    }
}