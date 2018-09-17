using System;
using System.Collections.Generic;
public class ProvinceState
{
    public Faction Owner { get; }
    public ProvinceUpgrades Upgrades { get; }
    public RallyTarget RallyTarget { get; }
    public Province Identifier { get; }

    private readonly HashSet<Tile> _tiles;
    public IEnumerable<Tile> Tiles { get { return _tiles; } }

    public ProvinceState(Faction owner, 
        ProvinceUpgrades upgrades, 
        RallyTarget rallyTarget,
        Province identifier,
        IEnumerable<Tile> tiles)
        :this(owner, 
             upgrades,
             identifier,
             tiles)
    {
        RallyTarget = rallyTarget;
    }
    public ProvinceState(Faction owner,
        ProvinceUpgrades upgrades,
        Province identifier,
        IEnumerable<Tile> tiles)
    {
        Owner = owner;
        Upgrades = upgrades;
        RallyTarget = new RallyTarget(this);
        Identifier = identifier;
        _tiles = new HashSet<Tile>(tiles);
    }
}
