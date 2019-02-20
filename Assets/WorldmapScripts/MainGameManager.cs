using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class MainGameManager
{
    private readonly List<GameTurnTransition> _turns = new List<GameTurnTransition>();
    public GameState CurrentState { get { return _turns[_turns.Count - 1].AfterEverything; } }
    private ProvinceNeighborsTable _provinceNeighbors;
    public GameTurnTransition this[int index] { get { return _turns[index]; } }
    public int TurnsCount { get { return _turns.Count; } }

    public GameDisplayManager DisplayManager { get; }
    public InteractionManager InteractionManager { get; }
    public UnityObjectManager ObjectManager { get; }
    
    public MainGameManager(GameSetup gameSetup)
    {
        MapAssetSet assetSet = gameSetup.MapDefinition.GetMapAssetSet();
        Map map = new Map(assetSet.MapSetup);

        IEnumerable<PlayerSetup> playerSetups = PlayerSetup.CreateFromMapDefinition(assetSet.MapSetup);
        GameTurnTransition initialState = GetInitialState(playerSetups, map);

        _turns.Add(initialState);
        _provinceNeighbors = new ProvinceNeighborsTable(CurrentState);

        ObjectManager = new UnityObjectManager(map, 
            assetSet,
            gameSetup.MapPrefab, 
            gameSetup.FactionPrefab, 
            gameSetup.OrderIndicatorPrefab,
            gameSetup.ScreenCanvas, 
            initialState.AfterEverything,
            playerSetups);
        InteractionManager = new InteractionManager(this, 
            map,
            gameSetup,
            ObjectManager, 
            playerSetups);
        DisplayManager = new GameDisplayManager(this, 
            gameSetup, 
            playerSetups.Select(item => item.Faction), 
            map,
            ObjectManager,
            InteractionManager.Factions,
            initialState.AfterEverything);
    }

    internal void Update(UiAethetics aethetics)
    {
        InteractionManager.Update(CurrentState, _provinceNeighbors);
        DisplayManager.DisplayGamestate();
        DisplayManager.UpdateUi(CurrentState, Time.deltaTime, aethetics, _provinceNeighbors);
    }

    private GameTurnTransition GetInitialState(IEnumerable<PlayerSetup> playerSetups, Map map)
    {
        IEnumerable<ProvinceState> provinces = GetInitialProvinces(playerSetups, map);
        GameState initialState = new GameState(provinces);
        MergeTable mergeTable = new MergeTable(new Dictionary<Province, Province>());
        return new GameTurnTransition(
            initialState,
            initialState,
            initialState,
            initialState,
            mergeTable,
            new War[0]);
    }

    private IEnumerable<ProvinceState> GetInitialProvinces(IEnumerable<PlayerSetup> playerSetups, Map map)
    {
        Dictionary<Tile, Faction> startingLocations = GetStartingLocations(playerSetups, map);

        List<ProvinceState> ret = new List<ProvinceState>();
        foreach (Tile tile in map)
        {
            Faction faction = Faction.Independent;
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

    private Dictionary<Tile, Faction> GetStartingLocations(IEnumerable<PlayerSetup> playerSetups, Map tiles)
    {
        Dictionary<Tile, Faction> ret = new Dictionary<Tile, Faction>();

        foreach (PlayerSetup player in playerSetups)
        {
            Tile tile = tiles.GetTile(player.StartRow, player.StartColumn);
            ret.Add(tile, player.Faction);
        }
        
        return ret;
    }

    public void AdvanceGame(GameTurnMoves moves)
    {
        GameTurnTransition newState = CurrentState.GetNextState(moves);
        _turns.Add(newState);

        IEnumerable<Faction> survivingFactions = newState.AfterEverything.GetSurvivingFactions();
        if(survivingFactions.Count() < 2)
        {
            HandleGameConclusion(survivingFactions);
        }
        DisplayManager.UpdateDisplayWrappers(CurrentState);
        _provinceNeighbors = new ProvinceNeighborsTable(CurrentState);
        InteractionManager.Factions.RenewBuilders(survivingFactions);

        InteractionManager.Timeline.SetToMax(TurnsCount);
    }

    private void HandleGameConclusion(IEnumerable<Faction> survivingFactions)
    {
        //TODO: Handle the completion of a game!
        throw new NotImplementedException();
    }
}
