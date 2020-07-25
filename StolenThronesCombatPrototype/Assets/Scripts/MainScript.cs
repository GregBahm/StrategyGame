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

        builder.AddLeft(GetKnights(), 0);
        builder.AddLeft(GetSwordsmen(), 0);
        builder.AddLeft(GetKnights(), 0);
        builder.AddLeft(GetSlinger(), 1);
        builder.AddLeft(GetSlinger(), 1);
        builder.AddLeft(GetLongbowmen(), 1);
        builder.AddLeft(GetLongbowmen(), 1);
        builder.AddLeft(GetCatapults(), 2);

        builder.AddRight(GetSwordsmen(), 0);
        builder.AddRight(GetPikemen(), 0);
        builder.AddRight(GetSwordsmen(), 0);
        builder.AddRight(GetOgres(), 0);
        builder.AddRight(GetCrossbowmen(), 1);
        builder.AddRight(GetCrossbowmen(), 1);
        builder.AddRight(GetDragon(), 2);
        builder.AddRight(GetBalista(), 2);
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
