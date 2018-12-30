public class ProvinceUpgrade
{
    public ProvinceUpgradeBlueprint Blueprint { get; }
    public Tile BaseTile { get; }

    public ProvinceUpgrade(ProvinceUpgradeBlueprint blueprint,
        Tile baseTile)
    {
        Blueprint = blueprint;
        BaseTile = baseTile;
    }
}
