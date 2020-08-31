using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class BattleStateSide : IReadOnlyList<BattleRank>
{
    private readonly IReadOnlyList<BattleRank> ranks;

    public bool StillFighting { get; }

    public IEnumerable<BattalionState> AllUnits { get { return ranks.SelectMany(item => item); } }

    public int Count => ranks.Count;

    public BattleRank this[int index] => ranks[index];

    private readonly IReadOnlyDictionary<BattalionIdentifier, int> positionsTable;

    public BattleStateSide(List<BattleRank> battalions)
    {
        ranks = battalions.AsReadOnly();
        StillFighting = GetIsStillFighting();
        positionsTable = GetPositionsTable();
    }

    private IReadOnlyDictionary<BattalionIdentifier, int> GetPositionsTable()
    {
        var ret = new Dictionary<BattalionIdentifier, int>();
        for (int i = 0; i < ranks.Count; i++)
        {
            foreach (BattalionState item in ranks[i])
            {
                ret.Add(item.Id, i);
            }
        }
        return ret;
    }

    public int GetPosition(BattalionIdentifier id)
    {
        return positionsTable[id];
    }

    private bool GetIsStillFighting()
    {
        return ranks.Any(unit => unit.StillFighting);
    }
    
    public BattleStateSide GetWithDefeatedRemoved()
    {
        return new BattleStateSide(ranks.Where(item => item.StillFighting).Select(item => item.GetWithDefeatedRemoved()).ToList());
    }

    public IEnumerator<BattleRank> GetEnumerator()
    {
        return ranks.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ranks.GetEnumerator();
    }
}
