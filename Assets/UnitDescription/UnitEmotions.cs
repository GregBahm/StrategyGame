public class UnitEmotions
{
    private readonly UnitMeteredAttribute _endurance;
    public UnitMeteredAttribute Endurance { get { return _endurance; } }

    private readonly UnitMeteredAttribute _moral;
    public UnitMeteredAttribute Moral { get { return _moral; } }

    public bool IsRouting { get; set; }

    public UnitEmotions()
    {
        _endurance = new UnitMeteredAttribute();
        _moral = new UnitMeteredAttribute();
    }

    public UnitEmotionsRecord AsReadonly()
    {
        return new UnitEmotionsRecord(Endurance.AsReadonly(),
            Moral.AsReadonly(),
            IsRouting);
    }
}