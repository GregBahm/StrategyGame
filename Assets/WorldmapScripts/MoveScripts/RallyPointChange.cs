public class RallyPointChange : PlayerMove
{
    public Province AlteredProvince { get; }
    public RallyTarget NewRallyTarget { get; }

    public RallyPointChange(Faction faction,
        Province alteredProvince,
        RallyTarget newRallyTarget) 
        : base(faction, MoveCategory.RallyPointChange)
    {
        AlteredProvince = alteredProvince;
        NewRallyTarget = newRallyTarget;
    }
}
