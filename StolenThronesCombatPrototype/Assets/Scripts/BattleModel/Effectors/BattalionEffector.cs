using System.Collections.Generic;

public abstract class BattalionEffector
{
    public abstract IEnumerable<BattalionStateModifier> GetEffect(BattalionState self, BattleStateSide allies, BattleStateSide enemies);

    public enum DamageType
    {
        Regular,
        ArmorPiercing
    }

    public enum MeleeAttackType
    {
        Regular,
        Charging
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
