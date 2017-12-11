public struct UnitMeteredAttributeRecord
{
    private readonly int _max;
    public int Max { get{ return _max; } }

    private readonly int _current;
    public int Current{ get{ return _current; } }

    public UnitMeteredAttributeRecord(int max, int current)
    {
        _max = max;
        _current = current;
    }
}
