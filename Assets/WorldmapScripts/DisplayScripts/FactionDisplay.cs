using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class FactionDisplay
{
    private readonly Vector3 IndicatorOffset = new Vector3(0, 1.35f, 0);
    private readonly UnityObjectManager _objectManager;
    private readonly FactionUnityObject _unityObject;
    private readonly FactionsInteractionManager _interactionManager;
    private readonly FactionInteraction _factionInteraction;
    private readonly ReadOnlyCollection<OrderIndicator> _orderIndicators;
    private readonly MapInteraction _mapInteraction;

    public Faction Faction { get; }

    public FactionDisplay(FactionInteraction factionInteraction,
        UnityObjectManager objectManager, 
        FactionsInteractionManager interactionManager,
        MapInteraction mapInteraction)
    {
        _objectManager = objectManager;
        _mapInteraction = mapInteraction;
        _unityObject = objectManager.GetUnityObject(factionInteraction.Faction); ;
        Faction = _unityObject.Faction;
        _unityObject.Text.text = Faction.Name;
        _unityObject.Text.color = Faction.Color;
        _interactionManager = interactionManager;
        _factionInteraction = factionInteraction;
        _orderIndicators = objectManager.GetOrderIndicatorsFor(factionInteraction.Faction).ToList().AsReadOnly();
    }

    public void UpdateUi(GameState state)
    {
        UpdateHud();
        UpdateOrderIndicators(state);
    }

    private void UpdateOrderIndicators(GameState state)
    {

        bool isSelected = _interactionManager.ActiveFaction == Faction;
        IIndicatableMove[] indicatables = _factionInteraction.IndicatableMoves.ToArray();
        for (int i = 0; i < FactionInteraction.MaxMoves; i++)
        {
            OrderIndicator indicator = _orderIndicators[i];
            bool exists = indicatables.Length > i;
            if (isSelected && exists)
            {
                indicator.gameObject.SetActive(true);
                IIndicatableMove indicatable = indicatables[i];
                PlaceIndicator(indicator, indicatable, state);
            }
            else
            {
                indicator.gameObject.SetActive(false);
            }
        }
    }

    private Vector3 GetIndicatorPositionFor(Province province, GameState state)
    {
        ProvinceState provinceState = state.GetProvinceState(province);
        return _objectManager.GetProvinceCenter(provinceState) + IndicatorOffset;
    }

    private void PlaceIndicator(OrderIndicator indicator, IIndicatableMove indicatable, GameState state)
    {
        Vector3 fromPos = GetIndicatorPositionFor(indicatable.From, state);
        Vector3 toPos = GetIndicatorPositionFor(indicatable.To, state);
        indicator.transform.position = fromPos;
        indicator.Target.position = toPos;
    }

    private void UpdateHud()
    {
        bool isSelected = _interactionManager.ActiveFaction == Faction;
        string displayText = GetDisplayText(isSelected);
        _unityObject.Text.text = displayText;
    }

    private string GetDisplayText(bool isSelected)
    {
        string ret = Faction.Name + " (" + _factionInteraction.RemainingMoves + " moves)";
        if (isSelected)
        {
            ret += " - Controlled";
        }
        return ret;
    }
}