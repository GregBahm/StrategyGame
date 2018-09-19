using System;
using System.Collections.Generic;
public class ProvinceState
{
    public Faction Owner { get; }
    public ProvinceUpgrades Upgrades { get; }
    public RallyTarget RallyTarget { get; }
    public Province Identifier { get; }
    public ArmyForces Forces { get; }

    private readonly HashSet<Tile> _tiles;
    public IEnumerable<Tile> Tiles { get { return _tiles; } }

    public ProvinceState(Faction owner, 
        ProvinceUpgrades upgrades, 
        RallyTarget rallyTarget,
        Province identifier,
        ArmyForces forces,
        IEnumerable<Tile> tiles)
        :this(owner, 
             upgrades,
             identifier,
             tiles)
    {
        Forces = forces;
        RallyTarget = rallyTarget;
    }
    public ProvinceState(Faction owner,
        ProvinceUpgrades upgrades,
        Province identifier,
        IEnumerable<Tile> tiles)
    {
        Owner = owner;
        Upgrades = upgrades;
        Forces = new ArmyForces();
        RallyTarget = new RallyTarget(identifier);
        Identifier = identifier;
        _tiles = new HashSet<Tile>(tiles);
    }

    public ArmyForces GetGeneratedArmyForces()
    {
        //TODO: Generate army forces from provincs
        return new ArmyForces();
    }
}
