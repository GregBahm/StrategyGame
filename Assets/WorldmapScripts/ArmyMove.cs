public class ArmyMove : PlayerMove
{
    public Army TargetArmy { get; }
    public Province TargetProvince { get; }

    public ArmyMove(Faction faction,
        Army targetArmy,
        Province targetProvince) 
        : base(faction, MoveCategory.ArmyMove)
    {
        TargetArmy = targetArmy;
        TargetProvince = targetProvince;
    }
}
