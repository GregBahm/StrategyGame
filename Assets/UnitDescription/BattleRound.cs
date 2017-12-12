using System.Collections.Generic;

public class BattleRound
{
    private readonly IEnumerable<UnitStateRecord> _units;
    public IEnumerable<UnitStateRecord> Units { get{ return _units; } }

    private readonly BattleStatus _status;
    public BattleStatus Status { get{ return _status; } }
    
    public BattleRound(IEnumerable<UnitStateRecord> units, BattleStatus status)
    {
        _units = units;
        _status = status;
    }
}
