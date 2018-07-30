public class RallyTarget
{
    public Army TargetArmy { get; }
    public Province TargetProvince { get; }

    public RallyTarget(Army targetArmy)
    {
        TargetArmy = targetArmy;
    }
    public RallyTarget(Province targetProvince)
    {
        TargetProvince = targetProvince;
    }
}
