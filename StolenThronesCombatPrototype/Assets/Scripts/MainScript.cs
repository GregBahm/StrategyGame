using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattalionTemplates;

public class MainScript : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float time;
    
    [SerializeField]
    private BattleVisualiser visualizer;

    private void Start()
    {
        Battle battle = CreateExampleBattle();
        visualizer.Initialize(battle, this);
    }

    private void Update()
    {
        visualizer.Display(time);
    }

    private Battle CreateExampleBattle()
    {
        BattleBuilder builder = new BattleBuilder();
        
        builder.LeftSide.Add(BattalionTemplates.GetSwordsmen(40));
        builder.LeftSide.Add(BattalionTemplates.GetKnights(10));
        builder.LeftSide.AddToNextRank(BattalionTemplates.GetSlingers(10));
        builder.LeftSide.Add(BattalionTemplates.GetLongbowmen(10));
        builder.LeftSide.AddToNextRank(BattalionTemplates.GetCatapults(4));

        builder.RightSide.Add(BattalionTemplates.GetSwordsmen(10));
        builder.RightSide.Add(BattalionTemplates.GetOgres(8));
        builder.RightSide.Add(BattalionTemplates.GetPikemen(20));
        builder.RightSide.AddToNextRank(BattalionTemplates.GetPikemen(10));
        builder.RightSide.Add(BattalionTemplates.GetCrossbowmen(10));
        builder.RightSide.AddToNextRank(BattalionTemplates.GetDragon(3));
        return builder.ToBattle();
    }
}