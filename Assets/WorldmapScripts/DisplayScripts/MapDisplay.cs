using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapDisplay
{
    private Material _skyMat;
    private Transform _mapUvs;

    public Map Map { get; }
    public Dictionary<Tile, TileDisplay> _dictionary;
    public IEnumerable<TileDisplay> TileDisplays { get { return _dictionary.Values; } }

    public static Vector2 AscendingTileOffset { get; } = new Vector2(1, -1.73f).normalized;
    
    public MapDisplay(GameSetup gameSetup, Map map, UnityObjectManager objectManager)
    {
        _skyMat = gameSetup.SkyMat;
        _mapUvs = gameSetup.MapUvs;
        Map = map;
        _dictionary = MakeTilesDictionary(map, objectManager);
        InitializeTiles();
    }

    public void UpdateUiState(MapInteraction mapInteraction, float timeDelta, UiAethetics aethetics)
    {
        SetStandardShaderProperties(aethetics);
        foreach (TileDisplay tile in TileDisplays)
        {
            tile.UpdateHighlighting(mapInteraction, aethetics.TransitionSpeed, timeDelta);
        }
    }

    private void InitializeTiles()
    {
        foreach (TileDisplay tile in TileDisplays)
        {
            tile.SetNeighbors(this);
        }
    }

    private Dictionary<Tile, TileDisplay> MakeTilesDictionary(Map tiles, UnityObjectManager objectManager)
    {
        Dictionary<Tile, TileDisplay> ret = new Dictionary<Tile, TileDisplay>();
        foreach (Tile tile in tiles)
        {
            TileUnityObject tileObject = objectManager.GetUnityObject(tile);
            TileDisplay display = CreateTileDisplay(tile, tileObject);
            ret.Add(tile, display);
        }
        return ret;
    }
    
    public TileDisplay GetTile(int row, int ascendingColumn)
    {
        Tile tile = Map.GetTile(row, ascendingColumn);
        return _dictionary[tile];
    }

    private TileDisplay CreateTileDisplay(Tile tile, TileUnityObject tileObject)
    {
        GameObject obj = tileObject.gameObject;
        TileDisplay ret = new TileDisplay(tile, this, obj); 
        obj.transform.position = GetProvincePosition(tile.Row, tile.AscendingColumn);
        return ret;
    }
    
    public Vector3 GetProvincePosition(int row, int ascendingColumn)
    {
        Vector2 ascendingOffset = AscendingTileOffset * ascendingColumn;
        Vector2 offset = ascendingOffset + new Vector2(row, 0);
        offset *= 2;
        return new Vector3(offset.x, 0, offset.y);
    }

    private void SetStandardShaderProperties(UiAethetics aethetics)
    {
        Shader.SetGlobalFloat("_TileMargin", aethetics.TileMargin);
        Shader.SetGlobalMatrix("_MapUvs", _mapUvs.worldToLocalMatrix);
        Shader.SetGlobalColor("_SideColor", aethetics.BackgroundColor);
        _skyMat.SetColor("_Tint", aethetics.BackgroundColor);
    }
}
