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
        
        builder.LeftFront.Add(UnitTemplates.GetPeasants());
        builder.LeftMid.Add(UnitTemplates.GetKnights());
        builder.LeftMid.Add(UnitTemplates.GetLongbowmen());
        builder.LeftRear.Add(UnitTemplates.GetCatapults());

        builder.RightFront.Add(UnitTemplates.GetSwordsmen());
        builder.RightMid.Add(UnitTemplates.GetOgres());
        builder.RightMid.Add(UnitTemplates.GetCrossbowman());
        builder.RightRear.Add(UnitTemplates.GetDragon());
        builder.RightRear.Add(UnitTemplates.GetBalista());
        battle = builder.ToBattle();
    }
}
