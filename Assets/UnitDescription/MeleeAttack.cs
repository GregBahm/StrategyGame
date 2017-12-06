public struct MeleeAttack
{
    private readonly int _attackPower;
    public int AttackPower { get{ return _attackPower; } }

    private readonly DamageType _damageType;
    public DamageType DamageType { get{ return _damageType; } }

    private readonly AreaOfEffectType _areaOfAffect;
    public AreaOfEffectType AreaOfEffect { get{ return _areaOfAffect; } }

    private readonly bool _chargeAttack;
    public bool ChargeAttack { get{ return _chargeAttack; } }

    public MeleeAttack(int attackPower,
        DamageType damageType,
        AreaOfEffectType areaOfEffect,
        bool chargeAttack)
    {
        _attackPower = attackPower;
        _damageType = damageType;
        _areaOfAffect = areaOfEffect;
        _chargeAttack = true;
    }
}
