using System;

public class ArmyState
{
    public Guid Identifier { get; }

    public Guid LocationId { get; }

    public ArmyForces Forces { get; }

    public bool Routed { get; }

    public ArmyState(Guid identifier, Guid locationId, ArmyForces forces, bool routed)
    {
        Identifier = identifier;
        LocationId = locationId;
        Forces = forces;
        Routed = routed;
    }
}
