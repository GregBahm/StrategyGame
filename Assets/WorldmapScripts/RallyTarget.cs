using System;

public class RallyTarget
{
    public Army TargetArmyId { get; }
    public Province TargetProvinceId { get; }

    public RallyTarget(ArmyState targetArmy)
    {
        TargetArmyId = targetArmy.Identifier;
    }
    public RallyTarget(ProvinceState targetProvince)
    {
        TargetProvinceId = targetProvince.Identifier;
    }
}
