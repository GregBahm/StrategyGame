public struct AttackRecord
{
    private readonly UnitState _attacker;
    public UnitState Attacker{ get{ return _attacker; } }

    private readonly UnitState _attackee;
    public UnitState Attackee { get{ return _attackee; } }

    private readonly int _damage;
    public int Damage { get{ return _damage; } }

    public AttackRecord(UnitState attacker, UnitState attackee, int damage)
    {
        _attacker = attacker;
        _attackee = attackee;
        _damage = damage;
    }
}
