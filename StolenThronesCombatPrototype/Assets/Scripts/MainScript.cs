using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;

public class MainScript : MonoBehaviour
{
    private Battle battle;

    private void Start()
    {
        BattleBuilder builder = new BattleBuilder();
        
        builder.LeftFront.Add(BattalionTemplates.GetPeasants());
        builder.LeftMid.Add(BattalionTemplates.GetKnights());
        builder.LeftMid.Add(BattalionTemplates.GetLongbowmen());
        builder.LeftRear.Add(BattalionTemplates.GetCatapults());

        builder.RightFront.Add(BattalionTemplates.GetSwordsmen());
        builder.RightMid.Add(BattalionTemplates.GetOgres());
        builder.RightMid.Add(BattalionTemplates.GetCrossbowmen());
        builder.RightRear.Add(BattalionTemplates.GetDragon());
        builder.RightRear.Add(BattalionTemplates.GetBalista());
        battle = builder.ToBattle();
    }
}
