public class UnitOffenseRecord
{
    private readonly int _strength;
    public int Strength { get { return _strength; } }

    private readonly int _precision;
    public int Precision { get { return _precision; } }

    public UnitOffenseRecord(int strength, int precision)
    {
        _strength = strength;
        _precision = precision;
    }
}
