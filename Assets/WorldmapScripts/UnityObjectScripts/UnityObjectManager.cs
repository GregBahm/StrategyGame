using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class UnityObjectManager
{
    public readonly ReadOnlyDictionary<Faction, FactionUnityObject> _factions;
    public IEnumerable<FactionUnityObject> Factions { get { return _factions.Values; } }
    private readonly ReadOnlyDictionary<Tile, TileUnityObject> _tiles;
    public IEnumerable<TileUnityObject> Tiles { get { return _tiles.Values; } }
    private readonly ReadOnlyDictionary<Faction, IEnumerable<OrderIndicator>> _orderIndicators;

    public UnityObjectManager(Map map, 
        GameObject tilePrefab, 
        GameObject factionPrefab,
        GameObject orderIndicatorPrefab,
        Canvas hudCanvas,
        GameState initialState,
        IEnumerable<PlayerSetup> playerSetups)
    {
        _tiles = MakeTileObjects(map, tilePrefab);
        _factions = MakeFactionObjects(hudCanvas, factionPrefab, playerSetups);
        _orderIndicators = MakeOrderIndicators(orderIndicatorPrefab, playerSetups);
    }

    private ReadOnlyDictionary<Faction, IEnumerable<OrderIndicator>> MakeOrderIndicators(GameObject orderIndicatorPrefab, IEnumerable<PlayerSetup> playerSetups)
    {
        Dictionary<Faction, IEnumerable<OrderIndicator>> ret = new Dictionary<Faction, IEnumerable<OrderIndicator>>();
        foreach (Faction faction in playerSetups.Select(item => item.Faction))
        {
            List<OrderIndicator> indicators = new List<OrderIndicator>();
            for (int i = 0; i < FactionInteraction.MaxMoves; i++)
            {
                GameObject indicatorObject = GameObject.Instantiate(orderIndicatorPrefab);
                indicatorObject.name = faction.Name + " Indicator " + i;
                OrderIndicator indicatorScript = indicatorObject.GetComponent<OrderIndicator>();
                indicators.Add(indicatorScript);
            }
            ret.Add(faction, indicators);
        }
        return new ReadOnlyDictionary<Faction, IEnumerable<OrderIndicator>>(ret);
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

    public TileUnityObject GetUnityObject(Tile tile)
    {
        return _tiles[tile];
    }
    public FactionUnityObject GetUnityObject(Faction faction)
    {
        return _factions[faction];
    }
    public IEnumerable<OrderIndicator> GetOrderIndicatorsFor(Faction faction)
    {
        return _orderIndicators[faction];
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
    
    public Vector3 GetProvinceCenter(ProvinceState state)
    {
        Vector3 ret = Vector3.zero;
        int count = 0;
        foreach (Tile tile in state.Tiles)
        {
            TileUnityObject tileObj = _tiles[tile];
            ret += tileObj.transform.position;
            count++;
        }
        if (count > 0)
        {
            ret = ret / count;
        }
        return ret;
    }
}
