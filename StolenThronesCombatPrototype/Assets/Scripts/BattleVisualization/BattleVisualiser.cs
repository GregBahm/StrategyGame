using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using static BattalionTemplates;

public class BattleVisualiser : MonoBehaviour
{
    private int totalRounds;

    private BattleSideVisualizer leftSide;
    [SerializeField]
    private RectTransform leftSideContainer;

    private BattleSideVisualizer rightSide;
    [SerializeField]
    private RectTransform rightSideContainer;

    [SerializeField]
    private GameObject battalionVisualPrefab;

    [SerializeField]
    private GameObject rankPrefab;

    [SerializeField]
    private ArtBinding[] bindings;

    private Dictionary<BattalionType, ArtBinding> bindingsDictionary;
    private IEnumerable<BattalionVisualizer> unitVisualizers;
    private MainScript mainScript;


    public void Initialize(Battle battle, MainScript mainScript)
    {
        this.mainScript = mainScript;
        bindingsDictionary = bindings.ToDictionary(item => item.Type, item => item);
        this.totalRounds = battle.Progression.Count;
        leftSide = new BattleSideVisualizer(leftSideContainer, battle.Progression[0].InitialState.LeftSide, this);
        rightSide = new BattleSideVisualizer(rightSideContainer, battle.Progression[0].InitialState.RightSide, this);
        this.unitVisualizers = GetUnitVisualizers(battle);
    }

    private IEnumerable<BattalionVisualizer> GetUnitVisualizers(Battle battle)
    {
        Dictionary<BattalionIdentifier, BattalionVisualizer> dictionary = InitializeVisualizers(battle);
        for (int i = 0; i < battle.Progression.Count; i++)
        {
            BattleRound round = battle.Progression[i];

            IEnumerable<BattalionBattleState> allUnits = round.InitialState.LeftSide.Units.Concat(round.InitialState.RightSide.Units);
            foreach (BattalionBattleState unitState in allUnits)
            {
                dictionary[unitState.Id].InsertState(unitState, battle.Progression[i], i);
            }
        }
        return dictionary.Values;
    }

    private Dictionary<BattalionIdentifier, BattalionVisualizer> InitializeVisualizers(Battle battle)
    {
        Dictionary<BattalionIdentifier, BattalionVisualizer> ret = new Dictionary<BattalionIdentifier, BattalionVisualizer>();
        foreach (var item in leftSide.Ranks.SelectMany(item => item.Units))
        {
            ret.Add(item.Id, item);
        }
        foreach (var item in rightSide.Ranks.SelectMany(item => item.Units))
        {
            ret.Add(item.Id, item);
        }
        return ret;
    }
    
    public void Display(float normalizedTime)
    {
        foreach (BattalionVisualizer unit in unitVisualizers)
        {
            unit.Dispay(normalizedTime);
        }
    }

    internal GameObject CreateRankVisualizer()
    {
        return Instantiate(rankPrefab);
    }

    internal BattalionVisualizer CreateVisualsFor(BattalionBattleState state, RectTransform rect)
    {
        GameObject retObj = Instantiate(battalionVisualPrefab);
        BattalionVisualizer ret = retObj.GetComponent<BattalionVisualizer>();
        ret.Initialize(totalRounds, bindingsDictionary[state.Id.Type].Sprite, state.Id);
        retObj.transform.parent = rect;
        return ret;
    }
}

class BattleSideVisualizer
{
    private readonly BattleVisualiser mothership;

    public RectTransform Rect { get; }

    public ReadOnlyCollection<BattleRankVisualizer> Ranks { get; }

    public BattleSideVisualizer(RectTransform rect, BattleStateSide side, BattleVisualiser mothership)
    {
        this.mothership = mothership;
        Rect = rect;
        Ranks = CreateRanks(side).AsReadOnly();
    }

    private List<BattleRankVisualizer> CreateRanks(BattleStateSide side)
    {
        List<BattleRankVisualizer> ret = new List<BattleRankVisualizer>();
        for (int i = 0; i < side.Ranks.Count; i++)
        {
            GameObject rankPrefab = mothership.CreateRankVisualizer();
            rankPrefab.transform.parent = Rect;
            BattleRankVisualizer rank = CreateRankVisualizer(rankPrefab, side.Ranks[i]);
            ret.Add(rank);
        }
        return ret;
    }

    private BattleRankVisualizer CreateRankVisualizer(GameObject rankPrefab, IEnumerable<BattalionBattleState> battalions)
    {
        RectTransform rect = rankPrefab.GetComponent<RectTransform>();
        List<BattalionVisualizer> battalionVisualizers = new List<BattalionVisualizer>();
        foreach (BattalionBattleState state in battalions)
        {
            BattalionVisualizer visualizer = mothership.CreateVisualsFor(state, rect);
            battalionVisualizers.Add(visualizer);
        }
        return new BattleRankVisualizer(rect, battalionVisualizers);
    }
}

class BattleRankVisualizer
{
    public RectTransform Rect { get; }

    public IEnumerable<BattalionVisualizer> Units { get; }

    public BattleRankVisualizer(RectTransform rect, IEnumerable<BattalionVisualizer> units)
    {
        Units = units.ToList();
    }
}