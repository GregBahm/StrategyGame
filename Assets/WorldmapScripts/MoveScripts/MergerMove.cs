public class MergerMove : PlayerMove
{
    public ProvinceState GrowingProvince { get; }
    public ProvinceState AbsorbedProvince { get; }

    public MergerMove(Faction faction,
        ProvinceState growingProvince,
        ProvinceState absorbedProvince) 
        : base(faction, MoveCategory.ArmyMove)
    {
        GrowingProvince = growingProvince;
        AbsorbedProvince = absorbedProvince;
    }
    
}
