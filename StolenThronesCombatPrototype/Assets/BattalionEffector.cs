public abstract class BattalionEffector
{
    public abstract BattalionBattleEffects GetEffect(BattalionState self, BattleStageSide allies, BattleStageSide enemies);

    public enum DamageType
    {
        Regular,
        ArmorPiercing
    }
    
    public static BattalionAttribute GetDamageAttributeFor(DamageType damage)
    {
        switch (damage)
        {
            case DamageType.ArmorPiercing:
                return BattalionAttribute.ArmorPiercingDamage;
            case DamageType.Regular:
            default:
                return BattalionAttribute.Damage;
        }
    }
}
