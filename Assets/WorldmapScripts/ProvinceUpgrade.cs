public class ProvinceUpgrade
{
    public ProvinceUpgradeBlueprint Blueprint { get; }
    public TileBehaviour BaseTile { get; }
    public int Rotations { get; }
    public bool PrimaryUnitProducer { get; }

    public ProvinceUpgrade(ProvinceUpgradeBlueprint blueprint,
        TileBehaviour baseTile,
        int rotations,
        bool primaryUnitProducer)
    {
        Blueprint = blueprint;
        BaseTile = baseTile;
        Rotations = rotations;
        PrimaryUnitProducer = primaryUnitProducer;
    }
}
