using System.Collections.Generic;

public class GameLoopManager
{
    private readonly List<GameState> _turns;
    public GameState CurrentState { get { return _turns[_turns.Count - 1]; } }

    public GameLoopManager()
    {
        _turns = new List<GameState>();
    }
    
    public void AdvanceGame(GameTurnMoves moves)
    {
        GameState newState = CurrentState.GetNextState(moves);
        _turns.Add(newState);
        // TODO: Determine if the player is dead
        // TODO: Update board visuals
    }
}
