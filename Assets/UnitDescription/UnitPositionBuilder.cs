public class UnitPositionBuilder
{
    public int XPos { get; set; }
    
    public int YPos { get; set; }

    public UnitPosition AsReadonly()
    {
        return new UnitPosition(XPos, YPos);
    }
}
