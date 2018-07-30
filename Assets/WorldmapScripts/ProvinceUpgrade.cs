public class ProvinceUpgrade
{
    public ProvinceUpgradeBlueprint Blueprint { get; }
    public Tile BaseTile { get; }
    public int Rotations { get; }
    public bool PrimaryUnitProducer { get; }

    public ProvinceUpgrade(ProvinceUpgradeBlueprint blueprint,
        Tile baseTile,
        int rotations,
        bool primaryUnitProducer)
    {
        Blueprint = blueprint;
        BaseTile = baseTile;
        Rotations = rotations;
        PrimaryUnitProducer = primaryUnitProducer;
    }
}
