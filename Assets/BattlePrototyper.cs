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
            ret.Add(UnitTemplates.GetSwordsman(10, i * 3 + 40, UnitAllegiance.Defender, SwordsmanPrefab));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(12, i * 3 + 40, UnitAllegiance.Defender, SwordsmanPrefab));
        }
        for (int i = 0; i < 16; i++)
        {
            ret.Add(UnitTemplates.GetArcher(5, i * 2 + 10, UnitAllegiance.Defender, ArcherPrefab));
        }
        for (int i = 0; i < 12; i++)
        {
            ret.Add(UnitTemplates.GetKnight(8, i, UnitAllegiance.Defender, KnightPrefab));
        }
        for (int i = 0; i < 12; i++)
        {
            ret.Add(UnitTemplates.GetKnight(8, i + 100, UnitAllegiance.Defender, KnightPrefab));
        }
        return ret;
    }

    private List<UnitState> GetPrototypeAttackers()
    {
        List<UnitState> ret = new List<UnitState>();
        for (int i = 0; i < 20; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(90, i * 2 + 40, UnitAllegiance.Attacker, SwordsmanPrefab));
        }
        for (int i = 0; i < 20; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(92, i * 2 + 40, UnitAllegiance.Attacker, SwordsmanPrefab));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(95, i * 3 + 40, UnitAllegiance.Attacker, ArcherPrefab));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(97, i * 3 + 40, UnitAllegiance.Attacker, ArcherPrefab));
        }
        for (int i = 0; i < 16; i++)
        {
            ret.Add(UnitTemplates.GetTroll(85, i * 3 + 60, UnitAllegiance.Attacker, TrollPrefab));
        }
        return ret;
    }
}
