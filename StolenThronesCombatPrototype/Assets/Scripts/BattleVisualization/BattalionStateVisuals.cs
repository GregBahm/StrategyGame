using System.Linq;

public class BattalionStateVisuals
{
    public BattleSide Side { get; }
    public BattalionState State { get; }

    public BattalionStateVisuals(BattalionState state, BattleRound round)
    {
        State = state;
        Side = round.InitialState.LeftSide.Any(unit => unit.Id == state.Id) ? BattleSide.Left : BattleSide.Right;
    }
}
