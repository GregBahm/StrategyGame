using System;
using System.Collections.Generic;

public class Province
{
    public Faction Owner { get; }
    public ProvinceUpgrades Upgrades { get; }
    public RallyTarget RallyTarget { get; }
    public Guid Identifier { get; }

    private readonly HashSet<TileBehaviour> _tiles;
    public IEnumerable<TileBehaviour> Tiles { get { return _tiles; } }

    public Province(Faction owner, 
        ProvinceUpgrades upgrades, 
        RallyTarget rallyTarget,
        Guid identifier,
        IEnumerable<TileBehaviour> tiles)
    {
        Owner = owner;
        Upgrades = upgrades;
        RallyTarget = rallyTarget;
        Identifier = identifier;
        _tiles = new HashSet<TileBehaviour>(tiles);
    }
    public Province(Faction owner,
        ProvinceUpgrades upgrades,
        Guid identifier,
        IEnumerable<TileBehaviour> tiles)
    {
        Owner = owner;
        Upgrades = upgrades;
        RallyTarget = new RallyTarget(this);
        Identifier = identifier;
        _tiles = new HashSet<TileBehaviour>(tiles);
    }
}
public class Tile
{
    public int Row { get; }
    public int AscendingColumn { get; }

    public Tile(int row, int ascendingColumn)
    {
        Row = row;
        AscendingColumn = ascendingColumn;
    }
}
