using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackRecordDisplay
{
    private readonly BattleUnitDisplay _attacker;
    public BattleUnitDisplay Attacker { get{ return _attacker; } }

    private readonly BattleUnitDisplay _attackee;
    public BattleUnitDisplay Attackee { get{ return _attackee; } }
    
    private readonly AttackRecord _attackRecord;
    public AttackRecord AttackRecord { get{ return _attackRecord; } }

    public AttackRecordDisplay(AttackRecord record, BattleUnitDisplay attacker, BattleUnitDisplay attackee)
    {
        _attackRecord = record;
        _attacker = attacker;
        _attackee = attackee;
    }
}

public class AttackDisplayRound
{
    private readonly IEnumerable<AttackRecordDisplay> _records;

    public AttackDisplayRound(IEnumerable<AttackRecordDisplay> rounds)
    {
        _records = rounds;
    }

    internal void Display()
    {
        foreach (AttackRecordDisplay record in _records)
        {
            bool isAttacker = record.Attacker.Allegiance == UnitAllegiance.Attacker;
            Color lineColor = isAttacker ? Color.red : Color.blue;
            Vector3 offset = new Vector3(0, isAttacker ? 0 : .1f, 0);
            Debug.DrawLine(record.Attacker.UnitGameObject.transform.position + offset, record.Attackee.UnitGameObject.transform.position + offset, lineColor);
        }
    }
}

public class BattleDisplay
{
    private readonly int _battleRoundsCount;
    private readonly IEnumerable<BattleUnitDisplay> _units;
    public IEnumerable<BattleUnitDisplay> Units { get{ return _units; } }
    private readonly int _battleLength;
    private readonly AttackDisplayRound[] _attackDisplayRounds;

    public BattleDisplay(List<BattleRound> battleRounds)
    {
        _battleRoundsCount = battleRounds.Count;
        var unitDictionary = new Dictionary<UnitIdentification, List<UnitStateRecord>>();
        for (int i = 0; i < _battleRoundsCount; i++)
        {
            BattleRound round = battleRounds[i];
            foreach (UnitStateRecord unit in round.Units)
            {
                FilterUnit(i, unit, unitDictionary);
            }
        }
        _units = InitializeDisplayUnits(unitDictionary);
        _battleLength = _units.Max(item => item.StatesCount);
        _attackDisplayRounds = GetAttackRecords(battleRounds);
    }

    private AttackDisplayRound[] GetAttackRecords(List<BattleRound> battleRounds)
    {
        AttackDisplayRound[] ret = new AttackDisplayRound[_battleRoundsCount];
        Dictionary<UnitIdentification, BattleUnitDisplay> unitsDictionary = _units.ToDictionary(item => item.Identification, item => item);


        for (int i = 0; i < _battleRoundsCount; i++)
        {
            IEnumerable<AttackRecord> attackRecords = battleRounds[i].CombatLog;
            List<AttackRecordDisplay> displayRecords = new List<AttackRecordDisplay>();
            foreach (AttackRecord record in attackRecords)
            {
                BattleUnitDisplay attackerDisplay = unitsDictionary[record.Attacker];
                BattleUnitDisplay attackeeDisplay = unitsDictionary[record.Attackee];
                AttackRecordDisplay newDisplayRecord = new AttackRecordDisplay(record, attackerDisplay, attackeeDisplay);
                displayRecords.Add(newDisplayRecord);
            }
            AttackDisplayRound moneyMelon = new AttackDisplayRound(displayRecords);
            ret[i] = moneyMelon;
        }
        return ret;
    }

    private static IEnumerable<BattleUnitDisplay> InitializeDisplayUnits(Dictionary<UnitIdentification, List<UnitStateRecord>> unitsDictionary)
    {
        List<BattleUnitDisplay> ret = new List<BattleUnitDisplay>();
        foreach (var item in unitsDictionary)
        {
            ret.Add(new BattleUnitDisplay(item.Value));
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
        int index = Mathf.FloorToInt((_battleLength - 1) * normalTime);
        _attackDisplayRounds[index].Display();
    }
}
