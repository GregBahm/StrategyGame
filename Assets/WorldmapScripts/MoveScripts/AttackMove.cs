public class AttackMove : PlayerMove
{
    public Province SourceProvince { get; }
    public Province TargetProvince { get; }

    public AttackMove(Faction faction,
        Province sourceProvince,
        Province targetProvince) 
        : base(faction, MoveCategory.Attack)
    {
        SourceProvince = sourceProvince;
        TargetProvince = targetProvince;
    }
}
