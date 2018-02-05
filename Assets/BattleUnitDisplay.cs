using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class BattleUnitDisplay
{
    private GameObject _unitGameobject;
    private Material _unitMaterial;

    private readonly List<UnitStateRecord> _states;
    public int StatesCount { get{ return _states.Count; } }

    public BattleUnitDisplay(List<UnitStateRecord> states)
    {
        _states = states;
        _unitGameobject = GameObject.Instantiate(states.First().Identification.ArtPrefab);
        _unitMaterial = _unitGameobject.GetComponentInChildren<MeshRenderer>().material;

    }

    public void SetScale(float scale)
    {
        _unitGameobject.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void DisplayTime(float normalTime, int battleLength)
    {
        float stackParam = (battleLength - 1) * normalTime;
        float midParam = stackParam % 1;
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
            DisplayLerpedState(priorState, nextState, midParam);
        }
        SetUnitColor(priorState);
        _unitGameobject.name = GetName(priorState);
        bool shouldFlip = GetShouldFlip(priorState);
        _unitMaterial.SetFloat("_Flip", shouldFlip ? 0 : 1);
    }

    private bool GetShouldFlip(UnitStateRecord priorState)
    {
        bool basis = priorState.Allegiance == UnitAllegiance.Defender;
        if(priorState.Emotions.IsRouting)
        {
            basis = !basis;
        }
        return basis;
    }

    private string GetName(UnitStateRecord priorState)
    {
        return priorState.Allegiance + " " + priorState.Identification.Name + " hp:" + priorState.HitPoints.Current + "/" + priorState.HitPoints.Max; 
    }

    private void SetUnitColor(UnitStateRecord priorState)
    {
        float hpRatio = (float)priorState.HitPoints.Current / priorState.HitPoints.Max;
        Color color = Color.Lerp(Color.red, Color.white, hpRatio);
        _unitMaterial.SetColor("_Color", color);
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
        UnitLocation priorPosition = prior.Position;
        UnitLocation nextPosition = next.Position;
        float xPos = Mathf.Lerp(priorPosition.XPos, nextPosition.XPos, normalTime);
        float yPos = Mathf.Lerp(priorPosition.YPos, nextPosition.YPos, normalTime);
        Vector3 pos = new Vector3(xPos, 0, yPos);
        _unitGameobject.transform.position = pos;
    }
}