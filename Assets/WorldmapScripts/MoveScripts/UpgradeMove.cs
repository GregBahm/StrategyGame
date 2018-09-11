public class UpgradeMove : PlayerMove
{
    public ProvinceState AlteredProvince { get; }
    public ProvinceUpgrade Upgrade { get; }

    public UpgradeMove(Faction faction,
        ProvinceState alteredProvince,
        ProvinceUpgrade upgrade) 
        : base(faction, MoveCategory.Upgrade)
    {
        AlteredProvince = alteredProvince;
        Upgrade = upgrade;
    }
}
