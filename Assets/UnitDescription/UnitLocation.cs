public struct UnitLocation
{
    private readonly int _xPos;
    public int XPos { get { return _xPos; } }

    private readonly int _yPos;
    public int YPos { get { return _yPos; } }

    public UnitLocation(int xPos, int yPos)
    {
        _xPos = xPos;
        _yPos = yPos;
    }

    public override string ToString()
    {
        return "UnitLocation: " + XPos.ToString() + ", " + YPos.ToString();
    }

    public override bool Equals(object obj)
    {
        return obj is UnitLocation && this == (UnitLocation)obj;
    }
    public override int GetHashCode()
    {
        return _xPos ^ _yPos;
    }
    public static bool operator ==(UnitLocation x, UnitLocation y)
    {
        return x._xPos == y._xPos && x._yPos == y._yPos;
    }
    public static bool operator !=(UnitLocation x, UnitLocation y)
    {
        return !(x == y);
    }
}
