using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDisplayManager
{
    private readonly Dictionary<Army, ArmyDisplay> _armies;
    private readonly Dictionary<Province, ProvinceDisplay> _provinces;
    private readonly Worldmap _worldMap;

    public GameDisplayManager(Worldmap worldmap)
    {
        _armies = new Dictionary<Army, ArmyDisplay>();
        _provinces = new Dictionary<Province, ProvinceDisplay>();
        _worldMap = worldmap;
    }

    public void UpdateDisplayWrappers(GameState state)
    {
        foreach (ArmyState army in state.Armies)
        {
            if(!_armies.ContainsKey(army.Identifier))
            {
                ArmyDisplay displayer = new ArmyDisplay(this, army.Identifier);
                _armies.Add(army.Identifier, displayer);
            }
        }
        foreach (ProvinceState province in state.Provinces)
        {
            if (!_provinces.ContainsKey(province.Identifier))
            {
                ProvinceDisplay displayer = new ProvinceDisplay(province.Identifier, this);
                _provinces.Add(province.Identifier, displayer);
            }
        }
    }

    public void DisplayTurn(GameTurnTransition turn, float progression)
    {
        DisplayTimings timings = new DisplayTimings(progression);
        // First new units are generated

        // Then armies move
        UpdateArmies(turn, timings);

        // Then display upgrades
        // Then display mergers
        UpdateProvinces(turn, timings);

        // Then display rally state changes
        // Then move units towards rally points
    }

    internal TileDisplay GetTile(Tile tile)
    {
        return _worldMap.GetTile(tile.Row, tile.AscendingColumn);
    }

    private void UpdateProvinces(GameTurnTransition transiation, DisplayTimings timings)
    {
        foreach (ProvinceDisplay province in _provinces.Values)
        {
            province.DisplayProvince(transiation, timings);
        }
    }

    private void UpdateArmies(GameTurnTransition transiation, DisplayTimings progression)
    {
        Dictionary<Army, ArmyTurnTransition> transitions =
            transiation.ArmyTransitions.ToDictionary(transition => transition.StartingState.Identifier, transition => transition);

        foreach (ArmyDisplay displayer in _armies.Values)
        {
            if(transitions.ContainsKey(displayer.Identifier))
            {
                ArmyTurnTransition transition = transitions[displayer.Identifier];
                displayer.DisplayArmy(transition, progression);
            }
            else
            {
                displayer.SetArmyAsDead();
            }
        }
    }
}
