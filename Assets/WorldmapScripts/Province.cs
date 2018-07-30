using System;
using System.Collections.Generic;

public class Province
{
    public Faction Owner { get; }
    public ProvinceUpgrades Upgrades { get; }
    public RallyTarget RallyTarget { get; }

    private readonly HashSet<Tile> _tiles;

    public Province(Faction owner, 
        ProvinceUpgrades upgrades, 
        RallyTarget rallyTarget,
        params Tile[] tiles)
    {
        Owner = owner;
        Upgrades = upgrades;
        RallyTarget = rallyTarget;
        _tiles = new HashSet<Tile>(tiles);
    }
}
