public class ProvinceUpgrade
{
    public ProvinceUpgradeBlueprint Blueprint { get; }
    public TileDisplay BaseTile { get; }
    public int Rotations { get; }

    public ProvinceUpgrade(ProvinceUpgradeBlueprint blueprint,
        TileDisplay baseTile,
        int rotations)
    {
        Blueprint = blueprint;
        BaseTile = baseTile;
        Rotations = rotations;
    }
}
