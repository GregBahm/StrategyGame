using System;

public class RallyTarget
{
    public Guid? TargetArmyId { get; }
    public Guid? TargetProvinceId { get; }

    public RallyTarget(ArmyState targetArmy)
    {
        TargetArmyId = targetArmy.Identifier;
    }
    public RallyTarget(ProvinceState targetProvince)
    {
        TargetProvinceId = targetProvince.Identifier;
    }
}
