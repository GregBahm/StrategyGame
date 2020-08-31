using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleRank : IEnumerable<BattalionState>
{
    private readonly IReadOnlyList<BattalionState> battalions;

    public bool StillFighting { get; }

    public BattleRank(IEnumerable<BattalionState> battalions)
    {
        this.battalions = battalions.ToList().AsReadOnly();
        StillFighting = GetIsStillFighting();
    }
    
    private bool GetIsStillFighting()
    {
        return battalions.Any(unit => unit.IsAlive);
    }

    public BattleRank GetWithDefeatedRemoved()
    {
        return new BattleRank(battalions.Where(item => item.IsAlive));
    }

    public IEnumerator<BattalionState> GetEnumerator()
    {
        return battalions.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return battalions.GetEnumerator();
    }
}
