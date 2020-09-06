using System.Collections.Generic;

public abstract class BattalionEffector
{
    public abstract IEnumerable<BattalionStateModifier> GetEffect(BattalionBattleState self, BattleStateSide allies, BattleStateSide enemies);

    public enum DamageType
    {
        Regular,
        ArmorPiercing,
        Charge
    }

    public enum MeleeAttackType
    {
        Regular,
        
    }
    
    public static BattalionAttribute GetDamageAttributeFor(DamageType damage)
    {
        switch (damage)
        {
            case DamageType.Charge:
                return BattalionAttribute.ChargingDamage;
            case DamageType.ArmorPiercing:
                return BattalionAttribute.ArmorPiercingDamage;
            case DamageType.Regular:
            default:
                return BattalionAttribute.Damage;
        }
    }
}
