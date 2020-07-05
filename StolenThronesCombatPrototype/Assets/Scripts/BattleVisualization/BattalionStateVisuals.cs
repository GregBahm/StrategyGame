using System.Linq;

public class BattalionStateVisuals
{
    public BattleSide Side { get; }
    public BattlePosition Position { get; }
    public int PositionIndex { get; }
    public int GroupCount { get; }
    
    public BattalionState State { get; }

    public BattalionStateVisuals(BattalionState state, BattleRound round)
    {
        State = state;
        Side = round.InitialState.LeftSide.AllUnits.Any(unit => unit.Id == state.Id) ? BattleSide.Left : BattleSide.Right;
        BattleStageSide stagingSide = Side == BattleSide.Left ? round.InitialState.LeftSide : round.InitialState.RightSide;
        BattlePositionInfo positionInfo = stagingSide.GetPosition(state.Id);
        Position = positionInfo.InitialPosition;
        PositionIndex = positionInfo.IndexWithinPosition;
        GroupCount = Position == BattlePosition.Front ? (stagingSide.Front.Count) : (Position == BattlePosition.Mid ? stagingSide.Mid.Count : stagingSide.Rear.Count);
    }
}
