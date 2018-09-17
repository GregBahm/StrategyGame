using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public const int Rows = 20;
    public const int Columns = 20;

    private readonly List<GameTurnTransition> _turns = new List<GameTurnTransition>();
    public GameTurnTransition CurrentState { get { return _turns[_turns.Count - 1]; } }

    private GameDisplayManager _displayManager;
    private Worldmap _worldMap;

    public float GameTime;
    public GameObject TilePrefab;

    [Range(0, 1)]
    public float TileMargin;
    public Transform MapUvs;

    private void Start()
    {
        _worldMap = new Worldmap(this);
        _displayManager = new GameDisplayManager(_worldMap);
        GameTurnTransition initialState = GetInitialState();
        _turns.Add(initialState);
        _displayManager.UpdateDisplayWrappers(initialState.FinalState);
    }

    private GameTurnTransition GetInitialState()
    {
        IEnumerable<ProvinceState> provinces = GetInitialProvinces();
        IEnumerable<ArmyState> armies = new ArmyState[0];
        GameState initialState = new GameState(provinces, armies);
        return new GameTurnTransition(initialState, initialState, new ArmyTurnTransition[0]);
    }

    private IEnumerable<ProvinceState> GetInitialProvinces()
    {
        Faction unownedFaction = new Faction(Color.gray);
        Dictionary<Tile, Faction> startingLocations = GetStartingLocations();

        List<ProvinceState> ret = new List<ProvinceState>();
        foreach (Tile tile in _worldMap.Tiles.Select(item => item.Tile))
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

        Tile tileA = _worldMap.GetTile(0, 0).Tile;
        Faction factionA = new Faction(Color.red);
        ret.Add(tileA, factionA);

        Tile tileB = _worldMap.GetTile(10, 10).Tile;
        Faction factionB = new Faction(Color.blue);
        ret.Add(tileB, factionB);

        return ret;
    }

    public void AdvanceGame(GameTurnMoves moves)
    {
        GameTurnTransition newState = CurrentState.FinalState.GetNextState(moves);
        _turns.Add(newState);
        // TODO: Determine if a player is dead
        // TODO: Update board visuals
        _displayManager.UpdateDisplayWrappers(newState.FinalState);
    }

    private void Update()
    {
        GameTime = Mathf.Clamp(GameTime, 0, _turns.Count - 1);
        DisplayGamestate();
    }

    private void DisplayGamestate()
    {
        GameTurnTransition turn = _turns[Mathf.FloorToInt(GameTime)];
        float progression = GameTime % 1;
        _displayManager.DisplayTurn(turn, progression);
    }

    public GameTurnTransition GetTurn(int turn)
    {
        return _turns[turn];
    }
}
