using System;
using UnityEngine;
using UnityEngine.UI;

public class FactionDisplay
{
    private readonly FactionUnityObject _unityObject;
    private readonly FactionsInteractionManager _interactionManager;
    private readonly FactionInteraction _interaction;

    public Faction Faction { get; }

    public FactionDisplay(FactionUnityObject unityObject, 
        FactionsInteractionManager interactionManager)
    {
        _unityObject = unityObject;
        Faction = _unityObject.Faction;
        _unityObject.Text.text = Faction.Name;
        _unityObject.Text.color = Faction.Color;
        _interactionManager = interactionManager;
        _interaction = _interactionManager.GetFactionInteraction(Faction);

    }

    public void UpdateUi()
    {
        bool isSelected = _interactionManager.ActiveFaction == Faction;
        string displayText = GetDisplayText(isSelected);
        _unityObject.Text.text = displayText;
    }

    private string GetDisplayText(bool isSelected)
    {
        string ret = Faction.Name + " (" + _interaction.RemainingMoves + " moves)";
        if (isSelected)
        {
            ret += " - Controlled";
        }
        return ret;
    }
}