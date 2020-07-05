using System;
using System.Collections.Generic;
using System.Linq;

public class RangedAttackEffector : BattalionEffector
{
    private readonly RangeStyle style;
    private readonly int weaponStrength;
    private readonly int splashDamage;
    private readonly DamageType damageType;

    public RangedAttackEffector(int weaponStrength, 
        RangeStyle style = RangeStyle.Regular,
        DamageType damageType = DamageType.Regular,
        int splashDamage = 0)
    {
        this.weaponStrength = weaponStrength;
        this.style = style;
        this.damageType = damageType;
        this.splashDamage = splashDamage;
    }
    public override BattalionBattleEffects GetEffect(BattalionState self, BattleStageSide allies, BattleStageSide enemies)
    {
        BattalionEffectsBuilder builder = new BattalionEffectsBuilder(this);

        int reloadingState = self.GetAttribute(BattalionAttribute.ReloadingState);
        if(reloadingState > 0)
        {
            builder.Add(self.Id, BattalionAttribute.ReloadingState, -1);
        }
        else
        {
            BattlePosition position = allies.GetPosition(self.Id).EffectivePosition;
            if (position == BattlePosition.Mid ||
                (position == BattlePosition.Rear && style != RangeStyle.ShortRange))
            {
                DoAttack(builder, self, enemies);
            }
        }
        return builder.ToEffects();
    }

    private void DoAttack(BattalionEffectsBuilder builder, BattalionState self, BattleStageSide enemies)
    {
        BattalionState target = GetTarget(enemies);
        BattalionAttribute damageAttribute = GetDamageAttributeFor(damageType);
        builder.Add(target.Id, damageAttribute, weaponStrength);

        if(splashDamage > 0)
        {
            IEnumerable<BattalionState> splashTargets = GetSplashTargets(target, enemies);
            foreach (var item in splashTargets)
            {
                builder.Add(target.Id, damageAttribute, splashDamage);
            }
        }

        int reloadingSpeed = self.GetAttribute(BattalionAttribute.ReloadingSpeed);
        builder.Add(self.Id, BattalionAttribute.ReloadingState, reloadingSpeed);
    }

    private IEnumerable<BattalionState> GetSplashTargets(BattalionState target, BattleStageSide enemies)
    {
        BattlePosition targetPos = enemies.GetPosition(target.Id).EffectivePosition;
        foreach (BattalionState unit in enemies.AllUnits.Where(item => item != target))
        {
            BattlePosition unitPos = enemies.GetPosition(unit.Id).EffectivePosition;
            if(unitPos == targetPos)
            {
                yield return unit;
            }
        }
    }

    private BattalionState GetTarget(BattleStageSide enemies)
    {
        if(style == RangeStyle.Bombard)
        {
            return enemies.GetFirstOfRank(BattlePosition.Mid);
        }
        return enemies.GetFirstOfRank(BattlePosition.Front);
    }

    public enum RangeStyle
    {
        ShortRange, // Can Attack from Mid, Targets Front
        Regular, // Can attack from Rear or Mid, Targets Front
        Bombard // Can attack from Rear or Mid, Targets Mid
    }
}