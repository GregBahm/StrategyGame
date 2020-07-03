using System.Collections.Generic;

public class BattleBuilder
{
    public List<BattalionState> LeftFront { get; } = new List<BattalionState>();
    public List<BattalionState> LeftMid { get; } = new List<BattalionState>();
    public List<BattalionState> LeftRear { get; } = new List<BattalionState>();

    public List<BattalionState> RightFront { get; } = new List<BattalionState>();
    public List<BattalionState> RightMid { get; } = new List<BattalionState>();
    public List<BattalionState> RightRear { get; } = new List<BattalionState>();

    public Battle ToBattle()
    {
        BattleStageSide leftSide = new BattleStageSide(LeftRear, LeftMid, LeftFront);
        BattleStageSide rightSide = new BattleStageSide(RightRear, RightMid, RightFront);
        return new Battle(leftSide, rightSide);
    }
}