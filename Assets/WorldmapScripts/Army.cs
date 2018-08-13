using System;

public class Army
{
    public Guid Identifier { get; }

    public Guid LocationId { get; }

    public ArmyForces Forces { get; }

    public Army(Guid identifier, Guid locationId)
    {
        Identifier = identifier;
        LocationId = locationId;
        Forces = Forces;
    }
}
public class ArmyForces
{
    //TODO: Define army forces
}