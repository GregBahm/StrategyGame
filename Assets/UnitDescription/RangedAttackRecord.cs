public struct RangedAttackRecord
{
    private readonly int _attackPower;
    public int AttackPower { get{ return _attackPower; } }

    private readonly DamageType _damageType;
    public DamageType DamageType { get{ return _damageType; } }

    private readonly int _minimumRange;
    public int MinimumRange { get{ return _minimumRange; } }

    private readonly int _maximumRange;
    public int MaximumRange { get{ return _maximumRange; } }

    private readonly AreaOfEffectType _areaOfEffect;
    public AreaOfEffectType AreaOfEffect { get{ return _areaOfEffect; } }

    private readonly int _ammunition;
    public int Ammunition { get{ return _ammunition; } }

    public RangedAttackRecord(int attackPower,
        DamageType damageType,
        int minimumRange,
        int maximumRange,
        AreaOfEffectType areaOfEffect,
        int ammunition)
    {
        _attackPower = attackPower;
        _damageType = damageType;
        _minimumRange = minimumRange;
        _maximumRange = maximumRange;
        _areaOfEffect = areaOfEffect;
        _ammunition = ammunition;
    }
}
