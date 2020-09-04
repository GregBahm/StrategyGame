public class BattalionStateVisuals
{
    public BattalionBattleState State { get; }
    public int Position { get; }
    public BattleSideIdentifier Side { get; }

    public BattalionStateVisuals(BattalionBattleState state, BattleSideIdentifier side)
    {
        State = state;
        Side = side;
    }
}
