using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class UnityObjectManager
{
    public MapUnityObject MapObject { get; }
    public readonly ReadOnlyDictionary<Faction, FactionUnityObject> _factions;
    public IEnumerable<FactionUnityObject> Factions { get { return _factions.Values; } }
    private readonly ReadOnlyDictionary<Faction, IEnumerable<OrderIndicator>> _orderIndicators;

    public UnityObjectManager(Map map,
        MapAssetSet mapAssets,
        GameObject mapPrefab, 
        GameObject factionPrefab,
        GameObject orderIndicatorPrefab,
        Canvas hudCanvas,
        GameState initialState,
        IEnumerable<PlayerSetup> playerSetups)
    {
        MapObject = MakeMapGameObject(mapPrefab, mapAssets);
        _factions = MakeFactionObjects(hudCanvas, factionPrefab, playerSetups);
        _orderIndicators = MakeOrderIndicators(orderIndicatorPrefab, playerSetups);
    }

    private MapUnityObject MakeMapGameObject(GameObject mapPrefab, MapAssetSet mapAssets)
    {
        GameObject obj = GameObject.Instantiate(mapPrefab);
        MapUnityObject ret = obj.GetComponent<MapUnityObject>();
        ret.Initialize(mapAssets);
        return ret;
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
            ((RectTransform)gameObject.transform).offsetMax = new Vector2(0, -indexer * 10);

            ret.Add(playerSetup.Faction, obj);
            indexer++;
        }
        return new ReadOnlyDictionary<Faction, FactionUnityObject>(ret);
    }
    public FactionUnityObject GetUnityObject(Faction faction)
    {
        return _factions[faction];
    }
    public IEnumerable<OrderIndicator> GetOrderIndicatorsFor(Faction faction)
    {
        return _orderIndicators[faction];
    }
    
    public Vector2 GetProvinceCenter(ProvinceState state)
    {
        Vector2 ret = Vector2.zero;
        int count = 0;
        foreach (Tile tile in state.Tiles)
        {
            ret += tile.Center;
            count++;
        }
        if (count > 0)
        {
            ret = ret / count;
        }
        return ret;
    }
}
