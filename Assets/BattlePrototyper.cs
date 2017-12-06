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
        IEnumerable<UnitStateBuilder> attackers = GetPrototypeAttackers();
        IEnumerable<UnitStateBuilder> defenders = GetPrototypeDefenders();
        return new BattleResolver(attackers, defenders, battlefield);
    }
    
    private void DisplayBattleRound(BattleRound battleRound)
    {
        foreach (UnitState state in battleRound.AttackingUnits.Concat(battleRound.DefendingUnits))
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = new Vector3(state.Attributes.Position.XPos, 0, state.Attributes.Position.YPos);
            obj.name = state.Identification.Name;
        }
    }

    private List<UnitStateBuilder> GetPrototypeDefenders()
    {
        List<UnitStateBuilder> ret = new List<UnitStateBuilder>();
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(10, i * 3 + 40));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(5, i * 3 + 10));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetKnight(10, i * 3));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetKnight(10, i * 3 + 100));
        }
        return ret;
    }

    private List<UnitStateBuilder> GetPrototypeAttackers()
    {
        List<UnitStateBuilder> ret = new List<UnitStateBuilder>();
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(90, i * 3 + 40));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(95, i * 3 + 40));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(97, i * 3 + 40));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetTroll(85, i * 3 + 60));
        }
        return ret;
    }
}
