using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class BattleStageSide
{
    public IReadOnlyList<BattalionState> Rear { get; }
    public IReadOnlyList<BattalionState> Mid { get; }
    public IReadOnlyList<BattalionState> Front { get; }
    public IEnumerable<BattalionState> AllUnits { get; }

    private readonly IReadOnlyDictionary<BattalionIdentifier, BattlePositionInfo> positionsTable;

    public bool StillFighting { get; }

    public BattleStageSide(List<BattalionState> rear,
        List<BattalionState> mid,
        List<BattalionState> front)
    {
        Rear = rear.AsReadOnly();
        Mid = mid.AsReadOnly();
        Front = front.AsReadOnly();
        AllUnits = front.Concat(mid).Concat(rear).ToList().AsReadOnly();
        StillFighting = GetIsStillFighting();
        positionsTable = CreatePositionsTable();
    }

    private Dictionary<BattalionIdentifier, BattlePositionInfo> CreatePositionsTable()
    {
        Dictionary<BattalionIdentifier, BattlePositionInfo> ret = new Dictionary<BattalionIdentifier, BattlePositionInfo>();
        for (int i = 0; i < Front.Count; i++)
        {
            BattalionState unit = Front[i];
            BattlePositionInfo info = new BattlePositionInfo(BattlePosition.Front, BattlePosition.Front, i);
            ret.Add(unit.Id, info);
        }
        for (int i = 0; i < Mid.Count; i++)
        {
            BattalionState unit = Mid[i];
            BattlePosition effectivePosition = Front.Any() ? BattlePosition.Mid : BattlePosition.Front;
            BattlePositionInfo info = new BattlePositionInfo(BattlePosition.Mid, effectivePosition, i);
            ret.Add(unit.Id, info);
        }
        for (int i = 0; i < Rear.Count; i++)
        {
            BattalionState unit = Rear[i];
            BattlePosition effectivePosition = Front.Any() ? (Mid.Any() ? BattlePosition.Rear : BattlePosition.Mid) : BattlePosition.Front;
            BattlePositionInfo info = new BattlePositionInfo(BattlePosition.Rear, effectivePosition, i);
            ret.Add(unit.Id, info);
        }
        return ret;
    }

    private bool GetIsStillFighting()
    {
        return AllUnits.Any(unit => unit.IsAlive);
    }
    
    public BattlePositionInfo GetPosition(BattalionIdentifier battalionId)
    {
        if(positionsTable.ContainsKey(battalionId))
        {
            return positionsTable[battalionId];
        }
        throw new InvalidOperationException("Cannot GetPosition() of battalion because battalion is not in BattleStageSide");
    }

    public BattleStageSide GetWithDefeatedRemoved()
    {
        List<BattalionState> newRear = Rear.Where(item => item.IsAlive).ToList();
        List<BattalionState> newMid = Mid.Where(item => item.IsAlive).ToList();
        List<BattalionState> newFront = Front.Where(item => item.IsAlive).ToList();
        return new BattleStageSide(newRear, newMid, newFront);
    }

    public BattalionState GetFirstOfRank(BattlePosition position)
    {
        switch (position)
        {
            case BattlePosition.Rear:
                if (Rear.Any())
                {
                    return Rear.First();
                }
                if (Mid.Any())
                {
                    return Mid.First();
                }
                return Front.First();
            case BattlePosition.Mid:
                if (Mid.Any())
                {
                    return Mid.First();
                }
                if (Rear.Any())
                {
                    return Rear.First();
                }
                return Front.First();
            case BattlePosition.Front:
            default:
                if (Front.Any())
                {
                    return Front.First();
                }
                if (Mid.Any())
                {
                    return Mid.First();
                }
                return Rear.First();
        }
    }
}
