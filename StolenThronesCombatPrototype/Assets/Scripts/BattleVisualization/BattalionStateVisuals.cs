public class BattalionStateVisuals
{
    public BattalionState State { get; }
    public int Position { get; }
    public BattleSide Side { get; }

    public BattalionStateVisuals(BattalionState state, int position, BattleSide side)
    {
        State = state;
        Position = position;
        Side = side;
    }
}
