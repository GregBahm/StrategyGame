using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;

public class UnityObjectManager
{
    private readonly GameObject _armyPrefab;
    private readonly Dictionary<Army, ArmyUnityObject> _armies;
    private ReadOnlyDictionary<Tile, TileUnityObject> _tiles;
    public IEnumerable<TileUnityObject> Tiles { get { return _tiles.Values; } }

    public UnityObjectManager(Map map, GameObject tilePrefab, GameObject armyPrefab, GameState initialState)
    {
        _tiles = MakeTileObjects(map, tilePrefab);
        SetCollisionClusters(map);
        _armyPrefab = armyPrefab;
        _armies = new Dictionary<Army, ArmyUnityObject>();
        UpdateGameobjects(initialState);
    }

    private void SetCollisionClusters(Map map)
    {
        foreach (TileUnityObject obj in _tiles.Values)
        {
            obj.SetCollisionCluster(this, map);
        }
    }

    public TileUnityObject GetUnityObject(Tile tile)
    {
        return _tiles[tile];
    }
    public ArmyUnityObject GetUnityObject(Army army)
    {
        return _armies[army];
    }

    private ReadOnlyDictionary<Tile, TileUnityObject> MakeTileObjects(Map map, GameObject tilePrefab)
    {
        Dictionary<Tile, TileUnityObject> ret = new Dictionary<Tile, TileUnityObject>();
        foreach (Tile tile in map)
        {
            TileUnityObject display = CreateTileObject(tile, tilePrefab);
            ret.Add(tile, display);
        }
        return new ReadOnlyDictionary<Tile, TileUnityObject>(ret);
    }

    private TileUnityObject CreateTileObject(Tile tile, GameObject tilePrefab)
    {
        int descendingColumn = tile.Row + tile.AscendingColumn;
        string providenceName = string.Format("Providence {0} {1} {2}", tile.Row, tile.AscendingColumn, descendingColumn);
        GameObject obj = GameObject.Instantiate(tilePrefab);
        obj.name = providenceName;

        TileUnityObject ret = obj.GetComponent<TileUnityObject>();
        ret.Tile = tile;
        return ret;
    }

    private ArmyUnityObject CreateNewArmy(ArmyState army)
    {
        GameObject armyObject = GameObject.Instantiate(_armyPrefab);
        ArmyUnityObject ret = armyObject.GetComponentInChildren<ArmyUnityObject>();
        ret.Army = army.Identifier;
        return ret;
    }

    public void UpdateGameobjects(GameState state)
    {
        foreach (ArmyState army in state.Armies)
        {
            if (!_armies.ContainsKey(army.Identifier))
            {
                ArmyUnityObject unityObject = CreateNewArmy(army);
                _armies.Add(army.Identifier, unityObject);
            }
        }
    }
}
