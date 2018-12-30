public class MergerMove : PlayerMove
{
    public Province GrowingProvince { get; }
    public Province AbsorbedProvince { get; }

    public MergerMove(Faction faction,
        Province growingProvince,
        Province absorbedProvince) 
        : base(faction, MoveCategory.Attack)
    {
        GrowingProvince = growingProvince;
        AbsorbedProvince = absorbedProvince;
    }
    
}
