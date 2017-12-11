public class UnitMeteredAttribute
{
    public int Max { get; set; }

    public int Current { get; set; }

    public UnitMeteredAttributeRecord AsReadonly()
    {
        return new UnitMeteredAttributeRecord(Max, Current);
    }
}
