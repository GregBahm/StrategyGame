public class RallyPointChange : PlayerMove
{
    public ProvinceState AlteredProvince { get; }
    public RallyTarget NewRallyTarget { get; }

    public RallyPointChange(Faction faction,
        ProvinceState alteredProvince,
        RallyTarget newRallyTarget) 
        : base(faction, MoveCategory.RallyPointChange)
    {
        AlteredProvince = alteredProvince;
        NewRallyTarget = newRallyTarget;
    }
}
