using System.Collections.Generic;

public class GameTurnTransition
{
    public GameState InitialState { get; }
    public GameState FinalState { get; }

    public IEnumerable<ArmyTurnTransition> ArmyTransitions { get; }

    public GameTurnTransition(GameState initialState,
        GameState finalState,
        IEnumerable<ArmyTurnTransition> armyTransitions)
    {
        InitialState = initialState;
        FinalState = finalState;
        ArmyTransitions = armyTransitions;
    }
}
