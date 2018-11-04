using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FactionsHud
{
    public IEnumerable<FactionDisplay> Factions { get; }

    public FactionsHud(Canvas hudCanvas, GameObject factionsPrefab, IEnumerable<Faction> factions)
    {
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
        foreach (FactionDisplay faction  in Factions)
        {
            bool isControlled = faction == factionDisplay;
            faction.UpdateText(isControlled);
        }
    }
}
