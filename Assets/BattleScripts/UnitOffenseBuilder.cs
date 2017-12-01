public class UnitOffenseBuilder
{
    public int Strength { get; set; }

    public int Precision { get; set; }

    public int AttackAccuracy { get; set; }

    public UnitOffense AsReadonly()
    {
        return new UnitOffense(Strength, Precision, AttackAccuracy);
    }
}
