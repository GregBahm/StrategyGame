public class UnitMeteredAttributeBuilder
{
    public int Max { get; set; }

    public int Current { get; set; }

    public UnitMeteredAttribute AsReadonly()
    {
        return new UnitMeteredAttribute(Max, Current);
    }
}
