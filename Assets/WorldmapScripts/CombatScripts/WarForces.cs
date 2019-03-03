using System;
using System.Collections.Generic;
using System.Linq;

public class WarForces
{
    public Faction Faction { get; }
    public IEnumerable<Army> Armies { get; }

    public WarForces(Faction faction, 
        IEnumerable<Army> armies)
    {
        Faction = faction;
        Armies = armies;
    }

    internal static WarForces CombineForces(IEnumerable<WarForces> forces, Faction faction)
    {
        IEnumerable<Army> armies = forces.SelectMany(item => item.Armies).ToArray();
        return new WarForces(faction, armies);
    }
}