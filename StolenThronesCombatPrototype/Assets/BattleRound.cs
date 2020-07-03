using System.Collections.Generic;

public class BattleRound
{
    public BattleState InitialState { get; }
    public IEnumerable<BattalionBattleEffects> Effects { get; }
    public BattleState FinalState { get; }
    public BattleRound(BattleState initialState, 
        IEnumerable<BattalionBattleEffects> effects, 
        BattleState finalState)
    {
        InitialState = initialState;
        Effects = effects;
        FinalState = finalState;
    }
}
