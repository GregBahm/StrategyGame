public class UnitOffense
{
    public int Strength { get; set; }

    public int Precision { get; set; }

    public int AttackAccuracy { get; set; }

    public UnitOffenseRecord AsReadonly()
    {
        return new UnitOffenseRecord(Strength, Precision, AttackAccuracy);
    }
}
