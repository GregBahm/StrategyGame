public struct AttackRecord
{
    private readonly UnitIdentification _attacker;
    public UnitIdentification Attacker { get{ return _attacker; } }

    private readonly UnitIdentification _attackee;
    public UnitIdentification Attackee { get{ return _attackee; } }

    private readonly int _damage;
    public int Damage { get{ return _damage; } }

    public AttackRecord(UnitIdentification attacker, UnitIdentification attackee, int damage)
    {
        _attacker = attacker;
        _attackee = attackee;
        _damage = damage;
    }
}
