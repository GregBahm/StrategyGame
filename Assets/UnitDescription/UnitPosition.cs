public class UnitPosition
{
    public int XPos { get; set; }
    
    public int YPos { get; set; }

    public UnitLocation AsReadonly()
    {
        return new UnitLocation(XPos, YPos);
    }
}
