public struct UnitEmotionsRecord
{
    private readonly UnitMeteredAttributeRecord _endurance;
    public UnitMeteredAttributeRecord Endurance { get { return _endurance; } }

    private readonly UnitMeteredAttributeRecord _moral;
    public UnitMeteredAttributeRecord Moral { get { return _moral; } }

    private readonly bool _isRouting;
    public bool IsRouting{ get{ return _isRouting; } }

    public UnitEmotionsRecord(UnitMeteredAttributeRecord endurance,
        UnitMeteredAttributeRecord moral,
        bool isRouting)
    {
        _endurance = endurance;
        _moral = moral;
        _isRouting = isRouting;
    }
}
