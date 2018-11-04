using UnityEngine;
using UnityEngine.UI;

public class FactionDisplay
{
    private readonly Text _textObject;

    public Faction Faction { get; }

    public FactionDisplay(Text textGameObject, Faction faction)
    {
        _textObject = textGameObject;
        _textObject.text = faction.Name;
        _textObject.color = faction.Color;
        Faction = faction;
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