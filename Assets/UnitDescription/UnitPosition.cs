public class UnitPosition
{
    public int XPos { get; set; }
    
    public int YPos { get; set; }

    public UnitPositionRecord AsReadonly()
    {
        return new UnitPositionRecord(XPos, YPos);
    }
}
