using System.Collections.Generic;

public class BattleRound
{
    public BattleState InitialState { get; }
    public IEnumerable<BattalionStateModifier> Effects { get; }
    public BattleState WithEffectsApplied { get; }
    public BattleState WithDefeatedRemoved { get; }
    public BattleRound(BattleState initialState, 
        IEnumerable<BattalionStateModifier> effects, 
        BattleState withEffectsApplied,
        BattleState withDefeatedRemoved)
    {
        InitialState = initialState;
        Effects = effects;
        WithEffectsApplied = withEffectsApplied;
        WithDefeatedRemoved = withDefeatedRemoved;
    }
}
