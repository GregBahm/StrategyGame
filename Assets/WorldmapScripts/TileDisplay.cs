using UnityEngine;

public class TileDisplay
{
    public GameObject GameObject { get; }

    public Material TileMat { get; }

    public Tile Tile { get; }

    public TileDisplay(Tile tile)
    {
        Tile = tile;
    }
}
