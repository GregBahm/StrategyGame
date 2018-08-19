using System;

public class Army
{
    public Guid Identifier { get; }

    public Guid LocationId { get; }

    public ArmyForces Forces { get; }

    public bool Routed { get; }

    public Army(Guid identifier, Guid locationId, ArmyForces forces, bool routed)
    {
        Identifier = identifier;
        LocationId = locationId;
        Forces = forces;
        Routed = routed;
    }
}
public class ArmyForces
{
    //TODO: Define army forces
}