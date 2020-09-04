using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class BattleStateSide
{
    public bool StillFighting { get; }

    public IEnumerable<BattalionBattleState> Units { get; }
    
    public BattleStateSide(List<List<BattalionState>> battalions)
    {
        Units = GetUnits();
        StillFighting = GetIsStillFighting();
    }

    private IEnumerable<BattalionBattleState> GetUnits()
    {
        throw new NotImplementedException();
    }

    private bool GetIsStillFighting()
    {
        return Units.Any(unit => unit.IsAlive);
    }
    
    public BattleStateSide GetWithDefeatedRemoved()
    {
        IEnumerable<BattalionBattleState> survivingRanks = Units.Where(item => item.IsAlive);
        IEnumerable<IGrouping<int, BattalionBattleState>> grouped = survivingRanks.GroupBy(item => item.Position);
        List<List<BattalionState>> listed = grouped.Select(item => item.Select(unit => (BattalionState)unit).ToList()).ToList();
        return new BattleStateSide(listed);
    }
}
