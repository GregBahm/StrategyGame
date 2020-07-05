using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
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

        builder.LeftFront.Add(BattalionTemplates.GetSlinger());
        builder.LeftMid.Add(BattalionTemplates.GetKnights());
        builder.LeftMid.Add(BattalionTemplates.GetLongbowmen());
        builder.LeftRear.Add(BattalionTemplates.GetCatapults());

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
