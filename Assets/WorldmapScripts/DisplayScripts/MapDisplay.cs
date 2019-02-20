using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapDisplay
{
    private Material _skyMat;
    private readonly MapUnityObject _mapUnityObject;

    public Map Map { get; }
    public Dictionary<Tile, TileDisplay> _dictionary;
    public IEnumerable<TileDisplay> TileDisplays { get { return _dictionary.Values; } }

    public static Vector2 AscendingTileOffset { get; } = new Vector2(1, -1.73f).normalized;
    
    public MapDisplay(GameSetup gameSetup, Map map, UnityObjectManager objectManager)
    {
        _mapUnityObject = objectManager.MapObject;
        _skyMat = gameSetup.SkyMat;
        Map = map;
        _dictionary = MakeTilesDictionary(map);
    }

    public void UpdateUiState(GameState gameState, 
        MapInteraction mapInteraction, 
        float timeDelta, 
        UiAethetics aethetics, 
        ProvinceNeighborsTable neighbors)
    {

        foreach (TileDisplay tile in TileDisplays)
        {
            tile.UpdateHighlighting(gameState, mapInteraction, aethetics.TransitionSpeed, timeDelta,  neighbors);
        }
        _mapUnityObject.UpdateTileStatesBuffer(TileDisplays);
    }

    private Dictionary<Tile, TileDisplay> MakeTilesDictionary(Map tiles)
    {
        Dictionary<Tile, TileDisplay> ret = new Dictionary<Tile, TileDisplay>();
        foreach (Tile tile in tiles)
        {
            TileDisplay display = CreateTileDisplay(tile);
            ret.Add(tile, display);
        }
        return ret;
    }
    
    public TileDisplay GetTile(int row, int ascendingColumn)
    {
        Tile tile = Map.GetTile(row, ascendingColumn);
        return _dictionary[tile];
    }

    private TileDisplay CreateTileDisplay(Tile tile)
    {
        return new TileDisplay(tile, this); 
    }
}
