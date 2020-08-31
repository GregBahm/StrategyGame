public class BattalionStateVisuals
{
    public BattalionState State { get; }
    public int Position { get; }
    public BattleSideIdentifier Side { get; }

    public BattalionStateVisuals(BattalionState state, int position, BattleSideIdentifier side)
    {
        State = state;
        Position = position;
        Side = side;
    }
}
