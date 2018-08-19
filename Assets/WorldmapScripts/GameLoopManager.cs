using System.Collections.Generic;

public class GameLoopManager
{
    public float DisplayedTurn;

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
        // TODO: Determine if a player is dead
        // TODO: Update board visuals
    }

    public void DisplayTurn()
    {
        // First new units are generated
        // Then armies move
            // Collision fights happen first
            // Then invasions and peaceful moves
        // 
    }
}

public class UnitDisplay
{

}

public class ArmyDisplay
{

}