using System;
using System.Collections.Generic;

public class BattlefieldState
{
    private readonly UnitLocation[] _attackerPositions;
    public UnitLocation[] AttackerPositions { get{ return _attackerPositions; } }
    private readonly UnitLocation[] _defenderPositions;
    public UnitLocation[] DefenderPositions { get{ return _defenderPositions; } }
    private readonly UnitLocation[] _neutralPositions;
    public UnitLocation[] NeutralPositions { get{ return _neutralPositions; } }
    private readonly UnitLocation[] _berzerkerPositions;
    public UnitLocation[] BerzerkerPositions { get{ return _berzerkerPositions; } }

    public IEnumerable<UnitLocation> AllUnits
    {
        get
        {
            foreach (UnitLocation dude in _attackerPositions)
            {
                yield return dude;
            }
            foreach (UnitLocation dude in _defenderPositions)
            {
                yield return dude;
            }
            foreach (UnitLocation dude in _neutralPositions)
            {
                yield return dude;
            }
            foreach (UnitLocation dude in _berzerkerPositions)
            {
                yield return dude;
            }
        }
    }

    public BattlefieldState(UnitLocation[] attackerPositions,
        UnitLocation[] defenderPositions,
        UnitLocation[] neutralPositions,
        UnitLocation[] berzerkerPositions)
    {
        _attackerPositions = attackerPositions;
        _defenderPositions = defenderPositions;
        _neutralPositions = neutralPositions;
        _berzerkerPositions = berzerkerPositions;
    }

    internal UnitState GetUnitAt(UnitLocation pos)
    {
        throw new NotImplementedException();
    }

    internal IEnumerable<UnitState> GetRangedTargetFor(UnitState unit, RangedAttack rangedAttack)
    {
        throw new NotImplementedException();
    }
}
