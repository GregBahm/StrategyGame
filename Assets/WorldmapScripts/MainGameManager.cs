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
    private readonly InteractionManager _interactionManager;

    private readonly TurnMovesProcessor _turnMovesProcessor;
    
    public MainGameManager(GameSetup gameSetup)
    {
        Worldmap worldMap = new Worldmap(gameSetup.TilePrefab, gameSetup.Rows, gameSetup.Columns);
        IEnumerable<PlayerSetup> playerSetups = GetPlayerSetups();
        WorldMap = worldMap;
        _displayManager = new GameDisplayManager(worldMap, gameSetup, playerSetups.Select(item => item.Faction));
        _interactionManager = new InteractionManager(gameSetup, worldMap);
        GameTurnTransition initialState = GetInitialState(playerSetups);
        _turns.Add(initialState);
        _displayManager.UpdateDisplayWrappers(initialState.PostMergersState);
        _turnMovesProcessor = new TurnMovesProcessor(this, playerSetups.Select(item => item.Faction), playerSetups.First().Faction);
    }

    internal void Update()
    {
        float gametime = _interactionManager.Timeline.MasterGameTime.Value;
        DisplayGamestate(gametime);
        _interactionManager.Update();
    }

    private GameTurnTransition GetInitialState(IEnumerable<PlayerSetup> playerSetups)
    {
        IEnumerable<ProvinceState> provinces = GetInitialProvinces(playerSetups);
        IEnumerable<ArmyState> armies = GetInitialArmies(playerSetups, provinces);
        GameState initialState = new GameState(provinces, armies);
        MergeTable mergeTable = new MergeTable(new Dictionary<Province, Province>());
        ArmyTurnTransition[] initialTransition = armies.Select(army => new ArmyTurnTransition(army, army, army.LocationId, army, army, true, false, false)).ToArray();
        return new GameTurnTransition(
            initialState,
            initialState,
            initialState,
            initialState,
            initialState,
            mergeTable,
            initialTransition);
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
            new PlayerSetup("Player A", Color.blue, -5, -5),
            new PlayerSetup("Player B", Color.red, 5, 5)
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

        IEnumerable<Faction> survivingFactions = newState.PostMergersState.GetSurvivingFactions();
        if(survivingFactions.Count() < 2)
        {
            HandleGameConclusion(survivingFactions);
        }
        _displayManager.UpdateDisplayWrappers(newState.PostMergersState);
        _turnMovesProcessor.RenewBuilders(survivingFactions);
    }

    private void HandleGameConclusion(IEnumerable<Faction> survivingFactions)
    {
        //TODO: Handle the completion of a game!
        throw new NotImplementedException();
    }

    public void DisplayGamestate(float gameTime)
    {
        GameTurnTransition turn = _turns[Mathf.FloorToInt(gameTime)];
        float progression = gameTime % 1;
        _displayManager.DisplayTurn(turn, progression);
    }
}
