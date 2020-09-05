using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class BattleStateSide
{
    public bool StillFighting { get; }

    public IReadOnlyList<IEnumerable<BattalionBattleState>> Ranks { get; }

    public IEnumerable<BattalionBattleState> Units { get; }
    
    public BattleStateSide(List<List<BattalionState>> battalions)
    {
        Units = GetUnits(battalions).ToList();
        Ranks = GetRanks();
        StillFighting = GetIsStillFighting();
    }

    private IReadOnlyList<IReadOnlyList<BattalionBattleState>> GetRanks()
    {
        IOrderedEnumerable<IGrouping<int, BattalionBattleState>> ordered = Units.GroupBy(item => item.Position).OrderBy(item => item.Key);
        return ordered.Select(item => item.ToList()).ToList().AsReadOnly();
    }

    private IEnumerable<BattalionBattleState> GetUnits(List<List<BattalionState>> battalions)
    {
        foreach (var item in battalions.SelectMany(rank => rank))
        {
            yield return item.ToBattleState(battalions);
        }
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
