using System.Linq;
using System.Collections.Generic;
public struct BattlefieldStateForMover
{
    private readonly UnitLocation[] _attackerPositions;
    public UnitLocation[] AttackerPositions { get { return _attackerPositions; } }
    private readonly UnitLocation[] _defenderPositions;
    public UnitLocation[] DefenderPositions { get { return _defenderPositions; } }
    private readonly UnitLocation[] _neutralPositions;
    public UnitLocation[] NeutralPositions { get { return _neutralPositions; } }
    private readonly UnitLocation[] _berzerkerPositions;
    public UnitLocation[] BerzerkerPositions { get { return _berzerkerPositions; } }

    public BattlefieldStateForMover(IEnumerable<UnitState> allUnits)
    {
        _attackerPositions = GetLocationsFromAllegiance(allUnits, UnitAllegiance.Attacker);
        _defenderPositions = GetLocationsFromAllegiance(allUnits, UnitAllegiance.Defender);
        _neutralPositions = GetLocationsFromAllegiance(allUnits, UnitAllegiance.Neutral);
        _berzerkerPositions = GetLocationsFromAllegiance(allUnits, UnitAllegiance.Berzerk);
    }

    private static UnitLocation[] GetLocationsFromAllegiance(IEnumerable<UnitState> units, UnitAllegiance allegiance)
    {
        return units.Where(item => item.Allegiance == allegiance).Select(item => item.Location).ToArray();
    }
}
