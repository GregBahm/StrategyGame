public struct UnitPositionRecord
{
    private readonly int _xPos;
    public int XPos { get { return _xPos; } }

    private readonly int _yPos;
    public int YPos { get { return _yPos; } }

    public UnitPositionRecord(int xPos, int yPos)
    {
        _xPos = xPos;
        _yPos = yPos;
    }
}
