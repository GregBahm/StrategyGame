public class MeleeAttackBuilder
{
    public int AttackPower { get; set; }
    public DamageType DamageType { get; set; }
    public AreaOfEffectType AreaOfEffect { get; set; }
    public bool ChargeAttack { get; set; }

    public MeleeAttack AsReadonly()
    {
        return new MeleeAttack(AttackPower, DamageType, AreaOfEffect, ChargeAttack);
    }
}