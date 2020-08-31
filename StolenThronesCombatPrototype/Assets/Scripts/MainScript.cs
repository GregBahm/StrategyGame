using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattalionTemplates;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    private ArtBinding[] bindings;

    private Dictionary<BattalionType, ArtBinding> bindingsDictionary;

    [SerializeField]
    [Range(0, 1)]
    private float time;

    [SerializeField]
    private GameObject battalionVisualPrefab;

    private BattleVisualiser visualizer;

    private void Start()
    {
        bindingsDictionary = bindings.ToDictionary(item => item.Type, item => item);
        Battle battle = CreateExampleBattle();
        visualizer = new BattleVisualiser(battle, this);
    }

    private void Update()
    {
        visualizer.Display(time);
    }

    private Battle CreateExampleBattle()
    {
        BattleBuilder builder = new BattleBuilder();

        builder.LeftSide.Add(BattalionTemplates.GetSwordsmen());
        builder.LeftSide.Add(BattalionTemplates.GetKnights());
        builder.LeftSide.Add(BattalionTemplates.GetKnights());
        builder.LeftSide.Add(BattalionTemplates.GetSlinger());
        builder.LeftSide.Add(BattalionTemplates.GetSlinger());
        builder.LeftSide.Add(BattalionTemplates.GetLongbowmen());
        builder.LeftSide.Add(BattalionTemplates.GetLongbowmen());
        builder.LeftSide.Add(BattalionTemplates.GetCatapults());

        builder.RightSide.Add(BattalionTemplates.GetSwordsmen());
        builder.RightSide.Add(BattalionTemplates.GetPikemen());
        builder.RightSide.Add(BattalionTemplates.GetSwordsmen());
        builder.RightSide.Add(BattalionTemplates.GetOgres());
        builder.RightSide.Add(BattalionTemplates.GetCrossbowmen());
        builder.RightSide.Add(BattalionTemplates.GetDragon());
        builder.RightSide.Add(BattalionTemplates.GetBalista());
        return builder.ToBattle();
    }

    public BattalionVisualizer CreateVisualsFor(BattalionType type, int totalRounds)
    {
        GameObject retObj = Instantiate(battalionVisualPrefab);
        BattalionVisualizer ret = retObj.GetComponent<BattalionVisualizer>();
        ret.Initialize(totalRounds, bindingsDictionary[type].Sprite);
        return ret;
    }
}
