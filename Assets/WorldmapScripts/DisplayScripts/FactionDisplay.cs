using System;
using UnityEngine;
using UnityEngine.UI;

public class FactionDisplay
{
    private readonly Text _textObject;

    public Faction Faction { get; }

    public bool IsSelected { get { return _interactionManager.PlayerFaction.Value == Faction; } }

    public string DisplayText
    {
        get
        {
            string ret = Faction.Name + " (" + _moveBuilder.RemainingMoves.Value + " moves)";
            if(IsSelected)
            {
                ret += " - Controlled";
            }
            return ret;
        }
    }

    private readonly PlayerMoveBuilder _moveBuilder;
    private readonly InteractionManager _interactionManager;

    public FactionDisplay(InteractionManager interactionManager, Text textGameObject, Faction faction)
    {
        _moveBuilder = interactionManager.TurnMovesProcessor.GetMoveBuilderFor(faction);
        _interactionManager = interactionManager;
        _textObject = textGameObject;
        _textObject.text = faction.Name;
        _textObject.color = faction.Color;
        Faction = faction;
        _moveBuilder.RemainingMoves.ValueChangedEvent += OnRemainingMovesChanged;
        _interactionManager.PlayerFaction.ValueChangedEvent += OnPlayerFactionChanged;
        UpdateText();
    }

    private void OnRemainingMovesChanged(int oldValue, int newValue)
    {
        UpdateText();
    }

    private void OnPlayerFactionChanged(Faction oldValue, Faction newValue)
    {
        UpdateText();
    }

    public void UpdateText()
    {
        _textObject.text = DisplayText;
    }
}