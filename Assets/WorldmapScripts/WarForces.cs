using System;
using System.Collections.Generic;

public class WarForces
{
    public Faction Faction { get; }

    public WarForces(Faction faction)
    {
        Faction = faction;
        // TODO: Define out the rest of war forces
    }

    internal static WarForces CombineForces(IEnumerable<WarForces> forces)
    {
        throw new NotImplementedException();
    }
}