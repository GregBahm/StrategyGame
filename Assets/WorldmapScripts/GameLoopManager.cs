using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public const int Rows = 20;
    public const int Columns = 20;

    private readonly List<GameTurnTransition> _turns;
    public GameTurnTransition CurrentState { get { return _turns[_turns.Count - 1]; } }

    private readonly GameDisplayManager _displayManager;

    public float GameTime;

    public MainGameManager(GameState initialState)
    {
        _turns = new List<GameTurnTransition>();
        IEnumerable<Tile> tiles = CreateTiles();
        _displayManager = new GameDisplayManager(initialState, tiles);
    }

    private IEnumerable<Tile> CreateTiles()
    {
        List<Tile> ret = new List<Tile>();
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                Tile tile = new Tile(row, column);
                ret.Add(tile);
            }
        }
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

    public GameTurnTransition GetTurn(int turn)
    {
        return _turns[turn];
    }
}
