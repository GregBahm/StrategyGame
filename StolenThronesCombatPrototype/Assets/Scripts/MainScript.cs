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

        builder.LeftUnits.Add(BattalionTemplates.GetSwordsmen());
        builder.LeftUnits.Add(BattalionTemplates.GetKnights());
        builder.LeftUnits.Add(BattalionTemplates.GetKnights());
        builder.LeftUnits.Add(BattalionTemplates.GetSlinger());
        builder.LeftUnits.Add(BattalionTemplates.GetSlinger());
        builder.LeftUnits.Add(BattalionTemplates.GetLongbowmen());
        builder.LeftUnits.Add(BattalionTemplates.GetLongbowmen());
        builder.LeftUnits.Add(BattalionTemplates.GetCatapults());

        builder.RightUnits.Add(BattalionTemplates.GetSwordsmen());
        builder.RightUnits.Add(BattalionTemplates.GetPikemen());
        builder.RightUnits.Add(BattalionTemplates.GetSwordsmen());
        builder.RightUnits.Add(BattalionTemplates.GetOgres());
        builder.RightUnits.Add(BattalionTemplates.GetCrossbowmen());
        builder.RightUnits.Add(BattalionTemplates.GetDragon());
        builder.RightUnits.Add(BattalionTemplates.GetBalista());
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
