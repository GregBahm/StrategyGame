public class ArmyMove : PlayerMove
{
    public ArmyState Army { get; }
    public ProvinceState TargetProvince { get; }

    public ArmyMove(Faction faction,
        ArmyState targetArmy,
        ProvinceState targetProvince) 
        : base(faction, MoveCategory.ArmyMove)
    {
        Army = targetArmy;
        TargetProvince = targetProvince;
    }
}
