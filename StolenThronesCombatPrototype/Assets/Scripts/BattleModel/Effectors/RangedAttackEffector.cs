using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

public class RangedAttackEffector : BattalionEffector
{
    private readonly int range;
    private readonly int weaponStrength;
    private readonly int splashDamage;
    private readonly DamageType damageType;

    public RangedAttackEffector(int weaponStrength, 
        int range = 1,
        DamageType damageType = DamageType.Regular,
        int splashDamage = 0)
    {
        this.weaponStrength = weaponStrength;
        this.range = range;
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
            if(IsWithinAttackingRange(self.Position))
            {
                DoAttack(builder, self, enemies);
            }
        }
        return builder.ToEffects();
    }

    private bool IsWithinAttackingRange(BattalionPosition position)
    {
        return position.X < range;
    }

    private void DoAttack(BattalionEffectsBuilder builder, BattalionState self, BattleStageSide enemies)
    {
        BattalionState target = enemies.GetTargetFor(self.Position);
        BattalionAttribute damageAttribute = GetDamageAttributeFor(damageType);
        builder.Add(target.Id, damageAttribute, weaponStrength);

        if(splashDamage > 0)
        {
            IEnumerable<BattalionState> splashTargets = GetSplashTargets(target, enemies).ToArray();
            foreach (BattalionState splashTarget in splashTargets)
            {
                builder.Add(splashTarget.Id, damageAttribute, splashDamage);
            }
        }

        int reloadingSpeed = self.GetAttribute(BattalionAttribute.ReloadingSpeed);
        builder.Add(self.Id, BattalionAttribute.ReloadingState, reloadingSpeed);
    }

    private IEnumerable<BattalionState> GetSplashTargets(BattalionState target, BattleStageSide enemies)
    {
        IEnumerable<BattalionPosition> positions = target.Position.GetAdjacentPositions().ToArray();
        HashSet<BattalionPosition> set = new HashSet<BattalionPosition>(positions);
        List<BattalionState> ret = new List<BattalionState>();
        foreach (var item in enemies)
        {
            if(set.Contains(item.Position))
            {
                ret.Add(item);
            }
        }
        return ret;
    }

}