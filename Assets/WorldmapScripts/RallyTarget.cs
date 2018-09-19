using System;

public class RallyTarget
{
    public Army TargetArmyId { get; }
    public Province TargetProvinceId { get; }

    public RallyTarget(Army targetArmy)
    {
        TargetArmyId = targetArmy;
    }
    public RallyTarget(Province targetProvince)
    {
        TargetProvinceId = targetProvince;
    }
}
