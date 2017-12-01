public struct UnitMeteredAttribute
{
    private readonly int _max;
    public int Max { get{ return _max; } }

    private readonly int _current;
    public int Current{ get{ return _current; } }

    public UnitMeteredAttribute(int max, int current)
    {
        _max = max;
        _current = current;
    }
}
