public class UpgradeMove : PlayerMove
{
    public Province AlteredProvince { get; }
    public ProvinceUpgrade Upgrade { get; }

    public UpgradeMove(Faction faction,
        Province alteredProvince,
        ProvinceUpgrade upgrade) 
        : base(faction, MoveCategory.Upgrade)
    {
        AlteredProvince = alteredProvince;
        Upgrade = upgrade;
    }
}
