using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapDisplay
{
    private readonly GameSetup _gameSetup;
    private readonly Map _map;
    public Dictionary<Tile, TileDisplay> _dictionary;
    public IEnumerable<TileDisplay> TileDisplays { get { return _dictionary.Values; } }

    public readonly Vector2 AscendingTileOffset = new Vector2(1, -1.73f).normalized;
    
    public MapDisplay(GameSetup gameSetup, Map map)
    {
        _map = map;
        _gameSetup = gameSetup;
        _dictionary = MakeTilesDictionary(gameSetup.TilePrefab, map);
        InitializeTiles();
    }

    public void UpdateUiState(MapInteraction mapInteraction)
    {
        SetStandardShaderProperties();
        foreach (TileDisplay tile in TileDisplays)
        {
            tile.UpdateHighlighting(mapInteraction, _gameSetup.HighlightDecaySpeed);
        }
    }

    private void InitializeTiles()
    {
        foreach (TileDisplay tile in TileDisplays)
        {
            tile.SetNeighbors(this);
        }
        foreach (TileDisplay tile in TileDisplays)
        {
            tile.SetCollisionCluster();
        }
    }

    private Dictionary<Tile, TileDisplay> MakeTilesDictionary(GameObject tilePrefab, Map tiles)
    {
        Dictionary<Tile, TileDisplay> ret = new Dictionary<Tile, TileDisplay>();
        foreach (Tile tile in tiles)
        {
            TileDisplay display = CreateTileDisplay(tile, tilePrefab);
            ret.Add(tile, display);
        }
        return ret;
    }
    
    public TileDisplay GetTile(int row, int ascendingColumn)
    {
        Tile tile = _map.GetTile(row, ascendingColumn);
        return _dictionary[tile];
    }

    private TileDisplay CreateTileDisplay(Tile tile, GameObject tilePrefab)
    {
        int descendingColumn = tile.Row + tile.AscendingColumn;
        string providenceName = string.Format("Providence {0} {1} {2}", tile.Row, tile.AscendingColumn, descendingColumn);
        GameObject obj = GameObject.Instantiate(tilePrefab);
        obj.name = providenceName;

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

    private void SetStandardShaderProperties()
    {
        Shader.SetGlobalFloat("_TileMargin", _gameSetup.TileMargin);
        Shader.SetGlobalMatrix("_MapUvs", _gameSetup.MapUvs.worldToLocalMatrix);
        Shader.SetGlobalColor("_SideColor", _gameSetup.BackgroundColor);
        _gameSetup.SkyMat.SetColor("_Tint", _gameSetup.BackgroundColor);
    }
}
