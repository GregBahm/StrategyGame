using System;
using System.Collections.Generic;
public class BattleRound
{
    private readonly IEnumerable<UnitState> _attackingUnits;
    public IEnumerable<UnitState> AttackingUnits { get{ return _attackingUnits; } }

    private readonly IEnumerable<UnitState> _defendingUnits;
    public IEnumerable<UnitState> DefendingUnits { get{ return _defendingUnits; } }

    private readonly BattleStatus _status;
    public BattleStatus Status { get{ return _status; } }
    
    public BattleRound(IEnumerable<UnitState> attackingUnits, IEnumerable<UnitState> defendingUnits, BattleStatus status)
    {
        _attackingUnits = attackingUnits;
        _defendingUnits = defendingUnits;
        _status = status;
    }

    public BattleRound GetNextRound()
    {
        throw new NotImplementedException();
    }
}
