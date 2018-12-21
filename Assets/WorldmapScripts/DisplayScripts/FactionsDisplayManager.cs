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
        FactionsInteractionManager interactionManager)
    {
        _mainGameManager = mainGameManager;
        Factions = CreateFactionsDisplay(objectManager, interactionManager);
    }

    private IEnumerable<FactionDisplay> CreateFactionsDisplay(UnityObjectManager objectManager, FactionsInteractionManager interactionManager)
    {
        List<FactionDisplay> ret = new List<FactionDisplay>();
        foreach (FactionInteraction faction in interactionManager.Factions)
        {
            FactionUnityObject unityObject = objectManager.GetUnityObject(faction.Faction);
            FactionDisplay factionDisplay = new FactionDisplay(unityObject, interactionManager);
            ret.Add(factionDisplay);
        }
        return ret;
    }

    internal void UpdateUi()
    {
        foreach (FactionDisplay display in Factions)
        {
            display.UpdateUi();
        }
    }
}
