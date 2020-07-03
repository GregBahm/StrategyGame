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
}
