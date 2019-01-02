using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FactionsDisplayManager
{
    private readonly MainGameManager _mainGameManager;
    public IEnumerable<FactionDisplay> Factions { get; }

    public FactionsDisplayManager(MainGameManager mainGameManager, 
        Canvas hudCanvas, 
        GameObject factionsPrefab, 
        UnityObjectManager objectManager,
        FactionsInteractionManager interactionManager,
        MapInteraction mapInteraction)
    {
        _mainGameManager = mainGameManager;
        Factions = CreateFactionsDisplay(objectManager, interactionManager, mapInteraction);
    }

    private IEnumerable<FactionDisplay> CreateFactionsDisplay(UnityObjectManager objectManager, FactionsInteractionManager interactionManager, MapInteraction mapInteraction)
    {
        List<FactionDisplay> ret = new List<FactionDisplay>();
        foreach (FactionInteraction faction in interactionManager.Factions)
        {
            FactionDisplay factionDisplay = new FactionDisplay(faction, objectManager, interactionManager, mapInteraction);
            ret.Add(factionDisplay);
        }
        return ret;
    }

    internal void UpdateUi(GameState state)
    {
        foreach (FactionDisplay display in Factions)
        {
            display.UpdateUi(state);
        }
    }
}
