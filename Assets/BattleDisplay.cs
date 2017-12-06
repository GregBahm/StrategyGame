using System.Collections.Generic;
public class BattleDisplay
{
    private readonly int _battleRoundsCount;
    private readonly IEnumerable<BattleUnitDisplay> _units;
    private readonly int _battleLength;

    public BattleDisplay(List<BattleRound> battleRounds)
    {
        _battleRoundsCount = battleRounds.Count;
        var attackersDictionary = new Dictionary<UnitIdentification, List<UnitState>>();
        var defendersDictionary = new Dictionary<UnitIdentification, List<UnitState>>();
        for (int i = 0; i < _battleRoundsCount; i++)
        {
            BattleRound round = battleRounds[i];
            foreach (UnitState unit in round.AttackingUnits)
            {
                FilterUnit(i, unit, attackersDictionary);
            }
            foreach (UnitState unit in round.DefendingUnits)
            {
                FilterUnit(i, unit, defendersDictionary);
            }
        }
        _units = InitializeDisplayUnits(attackersDictionary, defendersDictionary);
        _battleLength = _units.Max(item => item.StatesCount);
    }

    private IEnumerable<BattleUnitDisplay> InitializeDisplayUnits(Dictionary<UnitIdentification, List<UnitState>> attackersDictionary, 
        Dictionary<UnitIdentification, List<UnitState>> defendersDictionary)
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
        UnitState unit,
        Dictionary<UnitIdentification, List<UnitState>> dictionary)
    {
        if(dictionary.ContainsKey(unit.Identification))
        {
            dictionary[unit.Identification].Add(unit);
        }
        else
        {
            List<UnitState> stateList = new List<UnitState>();
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
