using UnityEngine;
using UnityEngine.UI;

public class FactionDisplay
{
    private readonly Text _textObject;

    public Faction Faction { get; }

    public FactionDisplay(ObservableProperty<Faction> playerFactionProp, Text textGameObject, Faction faction)
    {
        _textObject = textGameObject;
        _textObject.text = faction.Name;
        _textObject.color = faction.Color;
        Faction = faction;
        playerFactionProp.ValueChangedEvent += OnPlayerFactionChanged;
        UpdateText(playerFactionProp.Value == faction);
    }

    private void OnPlayerFactionChanged(Faction oldValue, Faction newValue)
    {
        UpdateText(Faction == newValue);
    }

    public void UpdateText(bool isSelected)
    {
        if(isSelected)
        {
            _textObject.text = Faction.Name + " - Controlled";
        }
        else
        {
            _textObject.text = Faction.Name;
        }
    }
}