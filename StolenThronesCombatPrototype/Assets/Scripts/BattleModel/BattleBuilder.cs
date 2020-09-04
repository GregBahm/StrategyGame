using System;
using System.Collections.Generic;
using System.Linq;

public class BattleBuilder
{
    public SideBuilder LeftSide { get; } = new SideBuilder();
    public SideBuilder RightSide { get; } = new SideBuilder();
    public Battle ToBattle()
    {
        BattleStateSide leftSideRet = LeftSide.ToSide();
        BattleStateSide rightSideRet = RightSide.ToSide();
        return new Battle(leftSideRet, rightSideRet);
    }

    public class SideBuilder
    {
        private readonly List<List<BattalionState>> units = new List<List<BattalionState>>();
        public SideBuilder()
        {
            units.Add(new List<BattalionState>());
        }
        public void Add(BattalionState state)
        {
            units.Last().Add(state);
        }
        public void AddToNextRank(BattalionState state)
        {
            units.Add(new List<BattalionState>());
            Add(state);
        }
        public BattleStateSide ToSide()
        {
            return new BattleStateSide(units);
        }
    }
}