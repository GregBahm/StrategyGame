using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDisplayManager
{
    private readonly Dictionary<Army, ArmyDisplay> _armies;
    private readonly Dictionary<Province, ProvinceDisplay> _provinces;
    private readonly MainGameManager _mainManager;
    private readonly GameObject _armyPrefab;
    private readonly FactionsDisplayManager _factions;
    public MapDisplay Map { get; }

    public GameDisplayManager(MainGameManager mainManager, 
        GameSetup gameSetup, 
        IEnumerable<Faction> factions, 
        Map map, 
        UnityObjectManager objectManager,
        FactionsInteractionManager interactionManager,
        GameState initialState)
    {
        _armies = new Dictionary<Army, ArmyDisplay>();
        _provinces = new Dictionary<Province, ProvinceDisplay>();
        _mainManager = mainManager;
        _armyPrefab = gameSetup.ArmyPrefab;
        _factions = new FactionsDisplayManager(mainManager, gameSetup.ScreenCanvas, gameSetup.FactionPrefab, objectManager, interactionManager);
        Map = new MapDisplay(gameSetup, map, objectManager);
        UpdateDisplayWrappers(initialState);
        DisplayGamestate(0);
    }

    public void OnTimeChanged(float newValue)
    {
        DisplayGamestate(newValue);
    }

    public void DisplayGamestate(float gameTime)
    {
        GameTurnTransition turn = _mainManager[Mathf.FloorToInt(gameTime)];
        float progression = gameTime % 1;
        DisplayTurn(turn, progression);
    }

    internal void UpdateUi(GameState gameState, float timeDelta, UiAethetics aethetics, ProvinceNeighborsTable neighbors)
    {
        UpdateUiAethetics(aethetics);
        Map.UpdateUiState(gameState, _mainManager.InteractionManager.Map, timeDelta, aethetics, neighbors);
        foreach (ArmyDisplay army in _armies.Values)
        {
            army.UpdateUi(_mainManager.InteractionManager.Map, aethetics, timeDelta);
        }
        _factions.UpdateUi();
    }

    private void UpdateUiAethetics(UiAethetics aethetics)
    {
        Shader.SetGlobalColor("_HoverColor", aethetics.HoverColor);
        Shader.SetGlobalColor("_SelectedColor", aethetics.SelectedColor);
        Shader.SetGlobalColor("_DraggingColor", aethetics.DraggingColor);
        Shader.SetGlobalColor("_ValidDraggedColor", aethetics.ValidDraggedColor);
        Shader.SetGlobalColor("_InvalidDraggedColor", aethetics.InvalidDraggedColor);
        Shader.SetGlobalColor("_TargetableColor", aethetics.TargetableColor);
    }

    public void UpdateDisplayWrappers(GameState state)
    {
        foreach (ArmyState army in state.Armies)
        {
            if(!_armies.ContainsKey(army.Identifier))
            {
                ArmyDisplay displayer = RegisterNewArmy(army);
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

    private ArmyDisplay RegisterNewArmy(ArmyState army)
    {
        ArmyUnityObject unityObject = _mainManager.ObjectManager.GetUnityObject(army.Identifier);
        return new ArmyDisplay(this, army.Identifier, unityObject.transform, unityObject.ArtContent);
    }

    public void DisplayTurn(GameTurnTransition turn, float progression)
    {
        DisplayTimings timings = new DisplayTimings(progression);

        UpdateTiles(turn, timings);
        UpdateArmies(turn, timings);
        UpdateProvinces(turn, timings);

        // Then display rally state changes
        // Then move units towards rally points
    }

    private void UpdateTiles(GameTurnTransition turn, DisplayTimings timings)
    {
        foreach (TileDisplay tileDisplay in Map.TileDisplays)
        {
            tileDisplay.DisplayTile(turn, timings);
        }
    }

    internal TileDisplay GetTile(Tile tile)
    {
        return Map.GetTile(tile.Row, tile.AscendingColumn);
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
