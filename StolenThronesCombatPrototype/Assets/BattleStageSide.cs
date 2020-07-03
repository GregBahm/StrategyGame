using System;
using System.Collections.Generic;
using System.Linq;

public class BattleStageSide
{
    public IReadOnlyList<BattalionState> Rear { get; }
    public IReadOnlyList<BattalionState> Mid { get; }
    public IReadOnlyList<BattalionState> Front { get; }
    public IEnumerable<BattalionState> AllUnits { get; }

    public bool StillFighting { get; }

    public BattleStageSide(List<BattalionState> rear,
        List<BattalionState> mid,
        List<BattalionState> front)
    {
        Rear = rear.AsReadOnly();
        Mid = mid.AsReadOnly();
        Front = front.AsReadOnly();
        AllUnits = rear.Concat(mid).Concat(front).ToList().AsReadOnly();
        StillFighting = GetIsStillFighting();
    }

    private bool GetIsStillFighting()
    {
        return AllUnits.Any(unit => unit.IsAlive);
    }

    public BattlePosition GetPosition(BattalionState battalion)
    {
        if(Front.Contains(battalion))
        {
            return BattlePosition.Front;
        }
        if(Mid.Contains(battalion))
        {
            if(Front.Any())
            {
                return BattlePosition.Mid;
            }
            return BattlePosition.Front;
        }
        if(Rear.Contains(battalion))
        {
            bool anyFront = Front.Any();
            bool anyMid = Mid.Any();
            if(anyFront && anyMid)
            {
                return BattlePosition.Rear;
            }
            if(anyFront || anyMid)
            {
                return BattlePosition.Mid;
            }
            return BattlePosition.Front;
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
