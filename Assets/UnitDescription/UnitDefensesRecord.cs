public struct UnitDefensesRecord
{
    private readonly int _armor;
    public int Armor { get { return _armor; } }

    private readonly ShieldStatus _shield;
    public ShieldStatus Shield { get { return _shield; } }

    private readonly int _regeneration;
    public int RegenerationPercent { get{ return _regeneration; } }

    public UnitDefensesRecord(int armor,
        ShieldStatus shield, 
        int regeneration)
    {
        _armor = armor;
        _shield = shield;
        _regeneration = regeneration;
    }
}
