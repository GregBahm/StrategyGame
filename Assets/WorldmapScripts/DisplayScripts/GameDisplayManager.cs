using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameDisplayManager
{
    private readonly Dictionary<Province, ProvinceDisplay> _provinces;
    private readonly MainGameManager _mainManager;
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
        _provinces = new Dictionary<Province, ProvinceDisplay>();
        _mainManager = mainManager;
        _factions = new FactionsDisplayManager(mainManager, 
            gameSetup.ScreenCanvas, 
            gameSetup.FactionPrefab, 
            objectManager, 
            interactionManager, 
            _mainManager.InteractionManager.Map);
        Map = new MapDisplay(gameSetup, map, objectManager);
        UpdateDisplayWrappers(initialState);
    }

    public void DisplayGamestate()
    {
        float gameTime = _mainManager.InteractionManager.Timeline.DisplayTime;
        GameTurnTransition turn = _mainManager[Mathf.FloorToInt(gameTime)];
        float progression = gameTime % 1;
        DisplayTurn(turn, progression);
    }

    internal void UpdateUi(GameState gameState, float timeDelta, UiAethetics aethetics, ProvinceNeighborsTable neighbors)
    {
        UpdateUiAethetics(aethetics);
        Map.UpdateUiState(gameState, _mainManager.InteractionManager.Map, timeDelta, aethetics, neighbors);
        _factions.UpdateUi(gameState);
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

        UpdateTiles(turn, timings);
        UpdateProvinces(turn, timings);
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
