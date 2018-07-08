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
    [Range(0, 10)]
    public float SpriteScale;

    public GameObject ArcherPrefab;
    public GameObject SwordsmanPrefab;
    public GameObject TrollPrefab;
    public GameObject KnightPrefab;

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
        foreach (BattleUnitDisplay unit in _battleDisplay.Units)
        {
            unit.SetScale(SpriteScale);
        }
    }

    private BattleResolver GetBattleResolver()
    {
        BattlefieldMover battlefield = new BattlefieldMover(MapCompute);
        IEnumerable<UnitState> attackers = GetPrototypeAttackers();
        IEnumerable<UnitState> defenders = GetPrototypeDefenders();
        List<UnitState> units = new List<UnitState>();
        units.AddRange(attackers);
        units.AddRange(defenders);
        return new BattleResolver(units, battlefield);
    }

    private List<UnitState> GetPrototypeDefenders()
    {
        List<UnitState> ret = new List<UnitState>();
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(20, i + 40, UnitAllegiance.Defender, SwordsmanPrefab));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(15, i + 40, UnitAllegiance.Defender, ArcherPrefab));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetKnight(20, i + 30, UnitAllegiance.Defender, KnightPrefab));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetKnight(20, i + 50, UnitAllegiance.Defender, KnightPrefab));
        }
        return ret;
    }

    private List<UnitState> GetPrototypeAttackers()
    {
        List<UnitState> ret = new List<UnitState>();
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(80, i + 40, UnitAllegiance.Attacker, SwordsmanPrefab));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(81, i + 40, UnitAllegiance.Attacker, SwordsmanPrefab));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(83, i + 40, UnitAllegiance.Attacker, ArcherPrefab));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(84, i + 40, UnitAllegiance.Attacker, ArcherPrefab));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetTroll(75, i + 44, UnitAllegiance.Attacker, TrollPrefab));
        }
        return ret;
    }
}
