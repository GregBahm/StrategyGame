public struct UnitDefensesRecord
{
    private readonly int _dodging;
    public int Dodging { get { return _dodging; } }

    private readonly int _armor;
    public int Armor { get { return _armor; } }

    private readonly ShieldStatus _shield;
    public ShieldStatus Shield { get { return _shield; } }

    private readonly int _regenerationPercent;
    public int RegenerationPercent { get{ return _regenerationPercent; } }

    public UnitDefensesRecord(int defense,
        int armor,
        ShieldStatus shield, 
        int regenerationPercent)
    {
        _dodging = defense;
        _armor = armor;
        _shield = shield;
        _regenerationPercent = regenerationPercent;
    }
}
