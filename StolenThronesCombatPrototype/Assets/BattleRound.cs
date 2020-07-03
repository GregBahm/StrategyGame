using System.Collections.Generic;

public class BattleRound
{
    public BattleState InitialState { get; }
    public IEnumerable<BattalionBattleEffects> Effects { get; }
    public BattleState WithEffectsApplied { get; }
    public BattleState WithDefeatedRemoved { get; }
    public BattleRound(BattleState initialState, 
        IEnumerable<BattalionBattleEffects> effects, 
        BattleState withEffectsApplied,
        BattleState withDefeatedRemoved)
    {
        InitialState = initialState;
        Effects = effects;
        WithEffectsApplied = withEffectsApplied;
        WithDefeatedRemoved = withDefeatedRemoved;
    }
}
