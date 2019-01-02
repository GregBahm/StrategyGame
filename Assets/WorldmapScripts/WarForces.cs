using System;
using System.Collections.Generic;
using System.Linq;

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
        WarForces[] forceArray = forces.ToArray();
        if(forceArray.Length == 1)
        {
            return forceArray[0];
        }

        throw new NotImplementedException();
    }
}