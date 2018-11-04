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
    public UnitProduction Production { get; }
    public DefensiveBenefit Defenses { get; }
    public Func<ProvinceState, bool> Requirements { get; }
    
    public ProvinceUpgradeBlueprint(
        ProvinceUpgradeDescription description,
        int tileFootprintSize,
        UnitProduction production,
        DefensiveBenefit defenses,
        Func<ProvinceState, bool> requirements)
    {
        Description = description;
        TileFootprintSize = tileFootprintSize;
        Production = production;
        Defenses = defenses;
        Requirements = requirements;
    }
    
    public static IEnumerable<ProvinceUpgradeBlueprint> AllBlueprints
    {
        get
        {
            yield return TestA;
            yield return TestB;
            yield return TestC;
        }
    }

    public static ProvinceUpgradeBlueprint TestA { get; }
    public static ProvinceUpgradeBlueprint TestB { get; }
    public static ProvinceUpgradeBlueprint TestC { get; }

    static ProvinceUpgradeBlueprint()
    {
        TestA = new ProvinceUpgradeBlueprint(new ProvinceUpgradeDescription("Test A"), 1, new UnitProduction(), new DefensiveBenefit(), state => true);
        TestB = new ProvinceUpgradeBlueprint(new ProvinceUpgradeDescription("Test B"), 1, new UnitProduction(), new DefensiveBenefit(), state => true);
        TestC = new ProvinceUpgradeBlueprint(new ProvinceUpgradeDescription("Test C"), 1, new UnitProduction(), new DefensiveBenefit(), state => true);
    }
}