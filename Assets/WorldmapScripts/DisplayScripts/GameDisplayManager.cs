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

    private void UpdateArmies(GameTurnTransition gameTurnTransition, DisplayTimings progression)
    {
        Dictionary<Army, ArmyTurnTransition> armyTransitions =
            gameTurnTransition.ArmyTransitions.ToDictionary(transition => transition.StartingState.Identifier, transition => transition);

        foreach (ArmyDisplay displayer in _armies.Values)
        {
            if(armyTransitions.ContainsKey(displayer.Identifier))
            {
                ArmyTurnTransition armyTransition = armyTransitions[displayer.Identifier];
                displayer.DisplayArmy(gameTurnTransition, armyTransition, progression);
            }
            else
            {
                displayer.SetArmyAsDead();
            }
        }
    }

    public Vector3 GetAverageTilePosition(IEnumerable<Tile> tiles)
    {
        Vector3 ret = Vector3.zero;
        int count = 0;
        foreach (Tile tile in tiles)
        {
            TileDisplay tileDisplay = GetTile(tile);
            ret += tileDisplay.GameObject.transform.position;
            count++;
        }
        return ret / count;
    }
}
