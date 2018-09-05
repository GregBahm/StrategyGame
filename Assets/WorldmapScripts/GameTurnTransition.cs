using System.Collections.Generic;

public class GameTurnTransition
{
    public GameState FinalState { get; }

    public IEnumerable<ArmyTurnTransition> ArmyTransitions { get; }

    public GameTurnTransition(GameState finalState,
        IEnumerable<ArmyTurnTransition> armyTransitions)
    {
        finalState = FinalState;
        ArmyTransitions = armyTransitions;
    }
}
