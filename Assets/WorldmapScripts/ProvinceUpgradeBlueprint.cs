using System;

public class ProvinceUpgradeBlueprint
{
    public int TileFootprintSize { get; }
    public UnitProduction Production { get; }
    public DefensiveBenefit Defenses { get; }
    public Func<ProvinceState, bool> Requirements { get; }
    
    public ProvinceUpgradeBlueprint(int tileFootprintSize,
        UnitProduction production,
        DefensiveBenefit defenses,
        Func<ProvinceState, bool> requirements)
    {
        TileFootprintSize = tileFootprintSize;
        Production = production;
        Defenses = defenses;
        Requirements = requirements;
    }
}
