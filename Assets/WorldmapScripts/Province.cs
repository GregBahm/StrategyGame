using System.Collections.Generic;

public class Province
{
    public Faction Owner { get; set; }

    public HashSet<Tile> Tiles;

    public Province(Faction owner, params Tile[] tiles)
    {
        Owner = owner;
        Tiles = new HashSet<Tile>(tiles);
    }
}
