using System;

public class RallyTarget
{
    public Guid? TargetArmyId { get; }
    public Guid? TargetProvinceId { get; }

    public RallyTarget(Army targetArmy)
    {
        TargetArmyId = targetArmy.Identifier;
    }
    public RallyTarget(Province targetProvince)
    {
        TargetProvinceId = targetProvince.Identifier;
    }
}
