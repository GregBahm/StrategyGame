using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FactionsHud
{
    private readonly MainGameManager _mainGameManager;
    public IEnumerable<FactionDisplay> Factions { get; }

    public FactionsHud(MainGameManager mainGameManager, Canvas hudCanvas, GameObject factionsPrefab, IEnumerable<Faction> factions)
    {
        _mainGameManager = mainGameManager;
        Factions = InitializeFactions(hudCanvas, factionsPrefab, factions);
    }

    private IEnumerable<FactionDisplay> InitializeFactions(Canvas hudCanvas, GameObject factionsPrefab, IEnumerable<Faction> factions)
    {
        List<FactionDisplay> ret = new List<FactionDisplay>();
        int indexer = 0;
        foreach (Faction faction in factions)
        {
            GameObject gameObject = GameObject.Instantiate(factionsPrefab, hudCanvas.transform);
            gameObject.name = faction.Name + " hud";
            Text textObject = gameObject.GetComponent<Text>();
            ((RectTransform) gameObject.transform).offsetMax = new Vector2(0, -indexer * 20);
            
            FactionDisplay factionDisplay = new FactionDisplay(textObject, faction);
            gameObject.GetComponent<Button>().onClick.AddListener(() => OnFactionClick(factionDisplay));

            ret.Add(factionDisplay);
            indexer++;
        }
        return ret;
    }

    private void OnFactionClick(FactionDisplay factionDisplay)
    {
        _mainGameManager.InteractionManager.PlayerFaction.Value = factionDisplay.Faction;
    }
}
