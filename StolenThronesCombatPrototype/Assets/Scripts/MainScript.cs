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

        builder.LeftFront.Add(BattalionTemplates.GetSwordsmen());
        builder.LeftFront.Add(BattalionTemplates.GetKnights());
        builder.LeftFront.Add(BattalionTemplates.GetKnights());
        builder.LeftMid.Add(BattalionTemplates.GetSlinger());
        builder.LeftMid.Add(BattalionTemplates.GetSlinger());
        builder.LeftMid.Add(BattalionTemplates.GetLongbowmen());
        builder.LeftMid.Add(BattalionTemplates.GetLongbowmen());
        builder.LeftRear.Add(BattalionTemplates.GetCatapults());

        builder.RightFront.Add(BattalionTemplates.GetSwordsmen());
        builder.RightFront.Add(BattalionTemplates.GetPikemen());
        builder.RightFront.Add(BattalionTemplates.GetSwordsmen());
        builder.RightMid.Add(BattalionTemplates.GetOgres());
        builder.RightMid.Add(BattalionTemplates.GetCrossbowmen());
        builder.RightRear.Add(BattalionTemplates.GetDragon());
        builder.RightRear.Add(BattalionTemplates.GetBalista());
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
