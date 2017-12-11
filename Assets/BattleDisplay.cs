using System.Collections.Generic;
using System.Linq;

public class BattleDisplay
{
    private readonly int _battleRoundsCount;
    private readonly IEnumerable<BattleUnitDisplay> _units;
    private readonly int _battleLength;

    public BattleDisplay(List<BattleRound> battleRounds)
    {
        _battleRoundsCount = battleRounds.Count;
        var attackersDictionary = new Dictionary<UnitIdentification, List<UnitStateRecord>>();
        var defendersDictionary = new Dictionary<UnitIdentification, List<UnitStateRecord>>();
        for (int i = 0; i < _battleRoundsCount; i++)
        {
            BattleRound round = battleRounds[i];
            foreach (UnitStateRecord unit in round.AttackingUnits)
            {
                FilterUnit(i, unit, attackersDictionary);
            }
            foreach (UnitStateRecord unit in round.DefendingUnits)
            {
                FilterUnit(i, unit, defendersDictionary);
            }
        }
        _units = InitializeDisplayUnits(attackersDictionary, defendersDictionary);
        _battleLength = _units.Max(item => item.StatesCount);
    }

    private IEnumerable<BattleUnitDisplay> InitializeDisplayUnits(Dictionary<UnitIdentification, List<UnitStateRecord>> attackersDictionary, 
        Dictionary<UnitIdentification, List<UnitStateRecord>> defendersDictionary)
    {
        List<BattleUnitDisplay> ret = new List<BattleUnitDisplay>();
        foreach (var item in attackersDictionary)
        {
            BattleUnitDisplay newUnit = new BattleUnitDisplay(item.Value, true);
            ret.Add(newUnit);
        }
        foreach (var item in defendersDictionary)
        {
            BattleUnitDisplay newUnit = new BattleUnitDisplay(item.Value, false);
            ret.Add(newUnit);
        }
        return ret;
    }

    private void FilterUnit(int index, 
        UnitStateRecord unit,
        Dictionary<UnitIdentification, List<UnitStateRecord>> dictionary)
    {
        if(dictionary.ContainsKey(unit.Identification))
        {
            dictionary[unit.Identification].Add(unit);
        }
        else
        {
            List<UnitStateRecord> stateList = new List<UnitStateRecord>();
            for (int i = 0; i < index - 1; i++)
            {
                stateList.Add(null);
            }
            stateList.Add(unit);
            dictionary.Add(unit.Identification, stateList);
        }
    }

    internal void DisplayTime(float normalTime)
    {
        foreach (BattleUnitDisplay unit in _units)
        {
            unit.DisplayTime(normalTime, _battleLength);
        }
    }
}
