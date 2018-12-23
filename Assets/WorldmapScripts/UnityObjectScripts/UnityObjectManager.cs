using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UnityObjectManager
{
    private readonly GameObject _armyPrefab;
    private readonly Dictionary<Army, ArmyUnityObject> _armies;
    public readonly ReadOnlyDictionary<Faction, FactionUnityObject> _factions;
    public IEnumerable<FactionUnityObject> Factions { get { return _factions.Values; } }
    private readonly ReadOnlyDictionary<Tile, TileUnityObject> _tiles;
    public IEnumerable<TileUnityObject> Tiles { get { return _tiles.Values; } }

    public UnityObjectManager(Map map, 
        GameObject tilePrefab, 
        GameObject armyPrefab,
        GameObject factionPrefab,
        Canvas hudCanvas,
        GameState initialState,
        IEnumerable<PlayerSetup> playerSetups)
    {
        _tiles = MakeTileObjects(map, tilePrefab);
        SetCollisionClusters(map);
        _armyPrefab = armyPrefab;
        _armies = new Dictionary<Army, ArmyUnityObject>();
        _factions = MakeFactionObjects(hudCanvas, factionPrefab, playerSetups);
        UpdateGameobjects(initialState);
    }

    private ReadOnlyDictionary<Faction, FactionUnityObject> MakeFactionObjects(Canvas hudCanvas,
        GameObject factionsPrefab,
        IEnumerable<PlayerSetup> playerSetups)
    {
        Dictionary<Faction, FactionUnityObject> ret = new Dictionary<Faction, FactionUnityObject>();
        int indexer = 0;
        foreach (PlayerSetup playerSetup in playerSetups)
        {
            GameObject gameObject = GameObject.Instantiate(factionsPrefab, hudCanvas.transform);
            FactionUnityObject obj = gameObject.GetComponent<FactionUnityObject>();
            obj.Faction = playerSetup.Faction;
            gameObject.name = playerSetup.Faction.Name + " hud";
            Text textObject = gameObject.GetComponent<Text>();
            ((RectTransform)gameObject.transform).offsetMax = new Vector2(0, -indexer * 10);

            ret.Add(playerSetup.Faction, obj);
            indexer++;
        }
        return new ReadOnlyDictionary<Faction, FactionUnityObject>(ret);
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
    public FactionUnityObject GetUnityObject(Faction faction)
    {
        return _factions[faction];
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
