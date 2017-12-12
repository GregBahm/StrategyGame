using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class BattlePrototyper : MonoBehaviour 
{
    public ComputeShader MapCompute;
    [Range(0, 1)]
    public float Timeline;

    private BattleResolver _battleResolver;
    private BattleDisplay _battleDisplay;
    
    private void Start()
    {
        _battleResolver = GetBattleResolver();
        List<BattleRound> rounds = _battleResolver.ResolveBattle();
        _battleDisplay = new BattleDisplay(rounds);
    }

    private void Update()
    {
        _battleDisplay.DisplayTime(Timeline);
    }

    private BattleResolver GetBattleResolver()
    {
        Battlefield battlefield = new Battlefield(1024, 1024, MapCompute);
        IEnumerable<UnitState> attackers = GetPrototypeAttackers();
        IEnumerable<UnitState> defenders = GetPrototypeDefenders();
        List<UnitState> units = new List<UnitState>();
        units.AddRange(attackers);
        units.AddRange(defenders);
        return new BattleResolver(units, battlefield);
    }
    
    private void DisplayBattleRound(BattleRound battleRound)
    {
        foreach (UnitStateRecord state in battleRound.Units)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = new Vector3(state.Position.XPos, 0, state.Position.YPos);
            obj.name = state.Identification.Name;
        }
    }

    private List<UnitState> GetPrototypeDefenders()
    {
        List<UnitState> ret = new List<UnitState>();
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(10, i * 3 + 40, UnitAllegiance.Defenders));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(5, i * 3 + 10, UnitAllegiance.Defenders));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetKnight(10, i * 3, UnitAllegiance.Defenders));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetKnight(10, i * 3 + 100, UnitAllegiance.Defenders));
        }
        return ret;
    }

    private List<UnitState> GetPrototypeAttackers()
    {
        List<UnitState> ret = new List<UnitState>();
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(90, i * 3 + 40, UnitAllegiance.Attackers));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(95, i * 3 + 40, UnitAllegiance.Attackers));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(97, i * 3 + 40, UnitAllegiance.Attackers));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetTroll(85, i * 3 + 60, UnitAllegiance.Attackers));
        }
        return ret;
    }
}
