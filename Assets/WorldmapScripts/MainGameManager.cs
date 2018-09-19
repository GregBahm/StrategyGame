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
        _displayManager = new GameDisplayManager(worldMap, bindings);
        GameTurnTransition initialState = GetInitialState();
        _turns.Add(initialState);
        _displayManager.UpdateDisplayWrappers(initialState.PostMergersState);
    }

    private GameTurnTransition GetInitialState()
    {
        IEnumerable<PlayerSetup> playerSetups = GetPlayerSetups();
        IEnumerable<ProvinceState> provinces = GetInitialProvinces(playerSetups);
        IEnumerable<ArmyState> armies = GetInitialArmies(playerSetups, provinces);
        GameState initialState = new GameState(provinces, armies);
        MergeTable mergeTable = new MergeTable(new Dictionary<Province, Province>());
        return new GameTurnTransition(
            initialState,
            initialState,
            initialState,
            initialState,
            initialState,
            mergeTable, 
            new ArmyTurnTransition[0]);
    }

    private IEnumerable<ArmyState> GetInitialArmies(IEnumerable<PlayerSetup> playerSetups, IEnumerable<ProvinceState> provinces)
    {
        List<ArmyState> ret = new List<ArmyState>();
        foreach (PlayerSetup setup in playerSetups)
        {
            Army army = new Army(setup.Faction);
            Province province = provinces.First(item => item.Owner == setup.Faction).Identifier;
            ArmyState armyState = new ArmyState(army, province, new ArmyForces(), false);
            ret.Add(armyState);
        }
        return ret;
    }

    private IEnumerable<PlayerSetup> GetPlayerSetups()
    {
        return new[]
        {
            new PlayerSetup("Player A", Color.cyan, 0, 0),
            new PlayerSetup("Player B", Color.red, 10, 10)
        };
    }

    private IEnumerable<ProvinceState> GetInitialProvinces(IEnumerable<PlayerSetup> playerSetups)
    {
        Faction unownedFaction = new Faction("Independent", Color.white);
        Dictionary<Tile, Faction> startingLocations = GetStartingLocations(playerSetups);

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

    private Dictionary<Tile, Faction> GetStartingLocations(IEnumerable<PlayerSetup> playerSetups)
    {
        Dictionary<Tile, Faction> ret = new Dictionary<Tile, Faction>();

        foreach (PlayerSetup player in playerSetups)
        {
            Tile tile = WorldMap.GetTile(player.StartRow, player.StartColumn).Tile;
            ret.Add(tile, player.Faction);
        }
        
        return ret;
    }

    public void AdvanceGame(GameTurnMoves moves)
    {
        GameTurnTransition newState = CurrentState.PostMergersState.GetNextState(moves);
        _turns.Add(newState);
        // TODO: Determine if a player is dead
        _displayManager.UpdateDisplayWrappers(newState.PostMergersState);
    }
    
    public void DisplayGamestate(float gameTime)
    {
        GameTurnTransition turn = _turns[Mathf.FloorToInt(gameTime)];
        float progression = gameTime % 1;
        _displayManager.DisplayTurn(turn, progression);
    }
}

public class PlayerSetup
{
    public Faction Faction { get; }
    public int StartRow { get; }
    public int StartColumn { get; }

    public PlayerSetup(string name, Color color, int startRow, int startColumn)
    {
        Faction = new Faction(name, color);
        StartRow = startRow;
        StartColumn = startColumn;
    }
}