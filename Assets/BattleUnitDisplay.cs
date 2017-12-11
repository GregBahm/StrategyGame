using System.Collections.Generic;
using UnityEngine;
public class BattleUnitDisplay
{
    private GameObject _unitGameobject;

    private readonly List<UnitStateRecord> _states;
    public int StatesCount { get{ return _states.Count; } }
    private readonly bool _attacker;

    public BattleUnitDisplay(List<UnitStateRecord> states, bool attacker)
    {
        _states = states;
        _attacker = attacker;
        _unitGameobject = GameObject.CreatePrimitive(PrimitiveType.Cube);
    }

    public void DisplayTime(float normalTime, int battleLength)
    {
        float stackParam = (battleLength - 1) * normalTime;
        int priorIndex = Mathf.FloorToInt(stackParam);
        int nextIndex = Mathf.CeilToInt(stackParam);

        if(priorIndex > (_states.Count - 1))
        {
            _unitGameobject.SetActive(false);
            return;
        }

        UnitStateRecord priorState = _states[priorIndex];
        UnitStateRecord nextState = _states[nextIndex];
        if (priorState == nextState || priorState == null || nextState == null || nextIndex > (_states.Count - 1))
        {
            DisplaySimpleState(priorState);
        }
        else
        {
            DisplayLerpedState(priorState, nextState, normalTime);
        }
    }

    private void DisplaySimpleState(UnitStateRecord unitState)
    {
        if(unitState == null || unitState.IsDefeated)
        {
            _unitGameobject.SetActive(false);
            return;
        }
        _unitGameobject.SetActive(true);
        Vector3 pos = new Vector3(unitState.Position.XPos, 0, unitState.Position.YPos);
        _unitGameobject.transform.position = pos;
    }

    private void DisplayLerpedState(UnitStateRecord prior, UnitStateRecord next, float normalTime)
    {
        if (prior == null || prior.IsDefeated)
        {
            _unitGameobject.SetActive(false);
            return;
        }

        _unitGameobject.SetActive(true);
        UnitPositionRecord priorPosition = prior.Position;
        UnitPositionRecord nextPosition = prior.Position;
        float xPos = Mathf.Lerp(priorPosition.XPos, nextPosition.XPos, normalTime);
        float yPos = Mathf.Lerp(priorPosition.YPos, nextPosition.YPos, normalTime);
        Vector3 pos = new Vector3(xPos, 0, yPos);
        _unitGameobject.transform.position = pos;
    }
}