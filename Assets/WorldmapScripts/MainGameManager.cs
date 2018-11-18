using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class MainGameManager
{
    private readonly List<GameTurnTransition> _turns = new List<GameTurnTransition>();
    public GameTurnTransition CurrentState { get { return _turns[_turns.Count - 1]; } }
    public GameTurnTransition this[int index] { get { return _turns[index]; } }
    public int TurnsCount { get { return _turns.Count; } }

    public GameDisplayManager DisplayManager { get; }
    public Worldmap WorldMap { get; }
    public InteractionManager InteractionManager { get; }
    
    public MainGameManager(GameSetup gameSetup)
    {
        WorldMap = new Worldmap(gameSetup.TilePrefab, gameSetup.Rows, gameSetup.Columns);

        IEnumerable<PlayerSetup> playerSetups = PlayerSetup.GetTestSetups();
        GameTurnTransition initialState = GetInitialState(playerSetups, WorldMap);

        _turns.Add(initialState);

        InteractionManager = new InteractionManager(this, gameSetup, WorldMap, playerSetups);
        DisplayManager = new GameDisplayManager(this, gameSetup, playerSetups.Select(item => item.Faction), initialState.PostMergersState);
    }

    internal void Update()
    {
        InteractionManager.Update();
    }

    private GameTurnTransition GetInitialState(IEnumerable<PlayerSetup> playerSetups, Worldmap worldmap)
    {
        IEnumerable<ProvinceState> provinces = GetInitialProvinces(playerSetups, worldmap);
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

    private IEnumerable<ProvinceState> GetInitialProvinces(IEnumerable<PlayerSetup> playerSetups, Worldmap worldmap)
    {
        Faction unownedFaction = new Faction("Independent", Color.white);
        Dictionary<Tile, Faction> startingLocations = GetStartingLocations(playerSetups, worldmap);

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

    private Dictionary<Tile, Faction> GetStartingLocations(IEnumerable<PlayerSetup> playerSetups, Worldmap worldmap)
    {
        Dictionary<Tile, Faction> ret = new Dictionary<Tile, Faction>();

        foreach (PlayerSetup player in playerSetups)
        {
            Tile tile = worldmap.GetTile(player.StartRow, player.StartColumn).Tile;
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
        DisplayManager.UpdateDisplayWrappers(newState.PostMergersState);
        InteractionManager.TurnMovesProcessor.RenewBuilders(survivingFactions);
    }

    private void HandleGameConclusion(IEnumerable<Faction> survivingFactions)
    {
        //TODO: Handle the completion of a game!
        throw new NotImplementedException();
    }
}
