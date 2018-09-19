using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class MainGameManager
{
    private readonly List<GameTurnTransition> _turns = new List<GameTurnTransition>();
    public GameTurnTransition CurrentState { get { return _turns[_turns.Count - 1]; } }
    public int TurnsCount { get { return _turns.Count; } }

    private readonly GameDisplayManager _displayManager;
    public Worldmap WorldMap { get; }

    public MainGameManager(GameBindings bindings, Worldmap worldMap)
    {
        WorldMap = worldMap;
        _displayManager = new GameDisplayManager(worldMap);
        GameTurnTransition initialState = GetInitialState();
        _turns.Add(initialState);
        _displayManager.UpdateDisplayWrappers(initialState.FinalState);
    }

    private GameTurnTransition GetInitialState()
    {
        IEnumerable<ProvinceState> provinces = GetInitialProvinces();
        IEnumerable<ArmyState> armies = new ArmyState[0];
        GameState initialState = new GameState(provinces, armies);
        MergeTable mergeTable = new MergeTable(new Dictionary<Province, Province>());
        return new GameTurnTransition(
            initialState,
            initialState,
            initialState,
            initialState,
            initialState,
            initialState,
            mergeTable, 
            new ArmyTurnTransition[0]);
    }

    private IEnumerable<ProvinceState> GetInitialProvinces()
    {
        Faction unownedFaction = new Faction(Color.gray);
        Dictionary<Tile, Faction> startingLocations = GetStartingLocations();

        List<ProvinceState> ret = new List<ProvinceState>();
        foreach (Tile tile in WorldMap.Tiles.Select(item => item.Tile))
        {
            Faction faction = unownedFaction;
            if(startingLocations.ContainsKey(tile))
            {
                faction = startingLocations[tile];
            }
            ProvinceState province = CreateInitialProvinceState(faction, tile);
            ret.Add(province);
        }
        return ret;
    }

    private ProvinceState CreateInitialProvinceState(Faction faction, Tile tile)
    {
        ProvinceUpgrades upgrades = new ProvinceUpgrades(new ProvinceUpgrade[0]);
        IEnumerable<Tile> tileSet = new[] { tile };
        return new ProvinceState(faction, upgrades, new Province(), tileSet);
    }

    private Dictionary<Tile, Faction> GetStartingLocations()
    {
        Dictionary<Tile, Faction> ret = new Dictionary<Tile, Faction>();

        Tile tileA = WorldMap.GetTile(0, 0).Tile;
        Faction factionA = new Faction(Color.red);
        ret.Add(tileA, factionA);

        Tile tileB = WorldMap.GetTile(10, 10).Tile;
        Faction factionB = new Faction(Color.blue);
        ret.Add(tileB, factionB);

        return ret;
    }

    public void AdvanceGame(GameTurnMoves moves)
    {
        GameTurnTransition newState = CurrentState.FinalState.GetNextState(moves);
        _turns.Add(newState);
        // TODO: Determine if a player is dead
        _displayManager.UpdateDisplayWrappers(newState.FinalState);
    }
    
    public void DisplayGamestate(float gameTime)
    {
        GameTurnTransition turn = _turns[Mathf.FloorToInt(gameTime)];
        float progression = gameTime % 1;
        _displayManager.DisplayTurn(turn, progression);
    }
}
