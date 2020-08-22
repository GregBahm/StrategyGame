using System.Collections.Generic;

public class BattleBuilder
{
    public List<BattalionState> LeftUnits { get; } = new List<BattalionState>();

    public List<BattalionState> RightUnits { get; } = new List<BattalionState>();

    public Battle ToBattle()
    {
        BattleStateSide leftSide = new BattleStateSide(LeftUnits);
        BattleStateSide rightSide = new BattleStateSide(RightUnits);
        return new Battle(leftSide, rightSide);
    }
}