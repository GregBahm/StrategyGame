public class ArmyMove : PlayerMove
{
    public Army Army { get; }
    public Province TargetProvince { get; }

    public ArmyMove(Faction faction,
        Army targetArmy,
        Province targetProvince) 
        : base(faction, MoveCategory.ArmyMove)
    {
        Army = targetArmy;
        TargetProvince = targetProvince;
    }
}
