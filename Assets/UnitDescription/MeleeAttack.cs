public class MeleeAttack
{
    public int AttackPower { get; set; }
    public DamageType DamageType { get; set; }
    public AreaOfEffectType AreaOfEffect { get; set; }
    public bool ChargeAttack { get; set; }
    private readonly UnitMeteredAttribute _cooldown;
    public UnitMeteredAttribute Cooldown { get{ return _cooldown; } }

    public MeleeAttack()
    {
        Cooldown = new UnitMeteredAttribute();
    }

    public MeleeAttackRecord AsReadonly()
    {
        return new MeleeAttackRecord(AttackPower, DamageType, AreaOfEffect, ChargeAttack);
    }
}