public class UnitOffenseRecord
{
    private readonly int _strength;
    public int Strength { get { return _strength; } }

    private readonly int _precision;
    public int Precision { get { return _precision; } }

    private readonly int _attackAccuracy;
    public int AttackAccuracy { get { return _attackAccuracy; } }

    public UnitOffenseRecord(int strength, int precision, int attackAccuracy)
    {
        _strength = strength;
        _precision = precision;
        _attackAccuracy = attackAccuracy;
    }
}
