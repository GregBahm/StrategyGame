using System;
using System.Collections.Generic;

public class Province
{
    public Faction Owner { get; }
    public ProvinceUpgrades Upgrades { get; }
    public RallyTarget RallyTarget { get; }
    public Guid Identifier { get; }

    private readonly HashSet<OldTileDisplay> _tiles;
    public IEnumerable<OldTileDisplay> Tiles { get { return _tiles; } }

    public Province(Faction owner, 
        ProvinceUpgrades upgrades, 
        RallyTarget rallyTarget,
        Guid identifier,
        IEnumerable<OldTileDisplay> tiles)
    {
        Owner = owner;
        Upgrades = upgrades;
        RallyTarget = rallyTarget;
        Identifier = identifier;
        _tiles = new HashSet<OldTileDisplay>(tiles);
    }
    public Province(Faction owner,
        ProvinceUpgrades upgrades,
        Guid identifier,
        IEnumerable<OldTileDisplay> tiles)
    {
        Owner = owner;
        Upgrades = upgrades;
        RallyTarget = new RallyTarget(this);
        Identifier = identifier;
        _tiles = new HashSet<OldTileDisplay>(tiles);
    }
}
