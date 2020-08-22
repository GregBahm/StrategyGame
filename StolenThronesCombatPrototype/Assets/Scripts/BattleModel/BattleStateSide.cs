using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class BattleStateSide : IReadOnlyList<BattalionState>
{
    private readonly IReadOnlyList<BattalionState> list;

    public bool StillFighting { get; }

    public int Count => list.Count;

    public BattalionState this[int index] => list[index];

    private readonly IReadOnlyDictionary<BattalionIdentifier, int> positionsTable;

    public BattleStateSide(List<BattalionState> battalions)
    {
        list = battalions.AsReadOnly();
        StillFighting = GetIsStillFighting();
        positionsTable = GetPositionsTable();
    }

    private IReadOnlyDictionary<BattalionIdentifier, int> GetPositionsTable()
    {
        var ret = new Dictionary<BattalionIdentifier, int>();
        for (int i = 0; i < list.Count; i++)
        {
            ret.Add(list[i].Id, i);
        }
        return ret;
    }

    public int GetPosition(BattalionIdentifier id)
    {
        return positionsTable[id];
    }

    private bool GetIsStillFighting()
    {
        return list.Any(unit => unit.IsAlive);
    }
    
    public BattleStateSide GetWithDefeatedRemoved()
    {
        return new BattleStateSide(list.Where(item => item.IsAlive).ToList());
    }

    public IEnumerator<BattalionState> GetEnumerator()
    {
        return list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return list.GetEnumerator();
    }
}
