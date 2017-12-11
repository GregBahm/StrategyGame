public class RangedAttack
{
    public int AttackPower { get; set; }
    public DamageType DamageType { get; set; }
    public int MinimumRange { get; set; }
    public int MaximumRange { get; set; }
    public AreaOfEffectType AreaOfEffect { get; set; }
    public int Ammunition { get; set; }
    
    public RangedAttackRecord AsReadonly()
    {
        return new RangedAttackRecord(AttackPower,
            DamageType,
            MinimumRange,
            MaximumRange,
            AreaOfEffect,
            Ammunition);
    }
}
