using System;
public class ArmyState
{
    public Army Identifier { get; }

    public Province LocationId { get; }

    public ArmyForces Forces { get; }

    public bool Routed { get; }

    public ArmyState(Army identifier, Province locationId, ArmyForces forces, bool routed)
    {
        Identifier = identifier;
        LocationId = locationId;
        Forces = forces;
        Routed = routed;
    }
}
