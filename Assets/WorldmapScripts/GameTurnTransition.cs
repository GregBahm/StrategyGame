using System.Collections.Generic;

public class GameTurnTransition
{
    public GameState InitialState { get; }
    public GameState FinalState { get; }
    public MergeTable MergeTable { get; }
    public IEnumerable<ArmyTurnTransition> ArmyTransitions { get; }

    public GameTurnTransition(GameState initialState,
        GameState finalState,
        MergeTable mergeTable,
        IEnumerable<ArmyTurnTransition> armyTransitions)
    {
        InitialState = initialState;
        FinalState = finalState;
        MergeTable = mergeTable;
        ArmyTransitions = armyTransitions;
    }
}
