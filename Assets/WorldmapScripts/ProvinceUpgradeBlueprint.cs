using System;
using System.Collections.Generic;

public class ProvinceUpgradeDescription
{
    public string Name { get; }
    public ProvinceUpgradeDescription(string name)
    {
        Name = name;
    }
}

public class ProvinceUpgradeBlueprint
{
    public ProvinceUpgradeDescription Description { get; }
    public int TileFootprintSize { get; }
    public Func<ProvinceState, bool> Requirements { get; }
    
    public ProvinceUpgradeBlueprint(
        ProvinceUpgradeDescription description,
        int tileFootprintSize,
        Func<ProvinceState, bool> requirements)
    {
        Description = description;
        TileFootprintSize = tileFootprintSize;
        Requirements = requirements;
    }
    
}