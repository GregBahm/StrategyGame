public class RangedAttackTemplate : BattalionEffector
{
    private readonly RangeStyle style;
    private readonly int weaponStrength;
    private readonly DamageType damageType;

    public RangedAttackTemplate(int weaponStrength, 
        RangeStyle style = RangeStyle.Regular,
        DamageType damageType = DamageType.Regular)
    {
        this.weaponStrength = weaponStrength;
        this.style = style;
        this.damageType = damageType;
    }
    public override BattalionBattleEffects GetEffect(BattalionState self, BattleStageSide allies, BattleStageSide enemies)
    {
        BattalionEffectsBuilder builder = new BattalionEffectsBuilder(this);
        BattlePosition position = allies.GetPosition(self);
        if (position == BattlePosition.Mid ||
            (position == BattlePosition.Rear && style != RangeStyle.ShortRange))
        {
            BattalionIdentifier target = GetTarget(enemies);
            BattalionAttribute damageAttribute = GetDamageAttributeFor(damageType);
            builder.Add(target, damageAttribute, weaponStrength);
        }
        return builder.ToEffects();
    }

    private BattalionIdentifier GetTarget(BattleStageSide enemies)
    {
        if(style == RangeStyle.Bombard)
        {
            return enemies.GetFirstOfRank(BattlePosition.Mid).Id;
        }
        return enemies.GetFirstOfRank(BattlePosition.Front).Id;
    }

    public enum RangeStyle
    {
        ShortRange, // Can Attack from Mid, Targets Front
        Regular, // Can attack from Rear or Mid, Targets Front
        Bombard // Can attack from Rear or Mid, Targets Mid
    }
}