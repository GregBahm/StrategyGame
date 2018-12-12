using System;
using UnityEngine;
using UnityEngine.UI;

public class FactionDisplay
{
    private readonly Text _textObject;

    public Faction Faction { get; }

    //public bool IsSelected { get { return _interactionManager.PlayerFaction.Value == Faction; } }

    public string DisplayText
    {
        get
        {
            return "TODO";
            //string ret = Faction.Name + " (" + _moveBuilder.RemainingMoves.Value + " moves)";
            //if(IsSelected)
            //{
            //    ret += " - Controlled";
            //}
            //return ret;
        }
    }

    public FactionDisplay(Text textGameObject, Faction faction)
    {
        _textObject = textGameObject;
        _textObject.text = faction.Name;
        _textObject.color = faction.Color;
        Faction = faction;
        UpdateText();
    }

    public void UpdateText()
    {
        _textObject.text = DisplayText;
    }
}