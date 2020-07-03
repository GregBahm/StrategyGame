using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


public class BattalionState
{
    public BattalionIdentifier Id { get; }

    public float RemainingHitpoints { get; }

    public float RemainingMoral { get; }

    public bool IsAlive { get { return RemainingHitpoints > 0 && RemainingMoral > 0; } }
    
    public IEnumerable<BattalionEffector> EffectSources { get; }

    public BattalionState(BattalionIdentifier id,
        float remainingHitpoints,
        float remainingMoral,
        IEnumerable<BattalionEffector> effectSources)
    {
        Id = id;
        RemainingHitpoints = remainingHitpoints;
        RemainingMoral = remainingMoral;
        EffectSources = effectSources;
    }

    public BattalionState GetWithModifiersApplied(IEnumerable<BattalionStateModifier> modifiers)
    {

    }

    public BattalionBattleEffects GetEffects(BattleStageSide allies, BattleStageSide enemies)
    {
        List<BattalionStateModifier> modifiers = new List<BattalionStateModifier>();
        List<BattalionSpawnEffect> spawns = new List<BattalionSpawnEffect>();
        
        foreach (BattalionEffector attack in EffectSources)
        {
            var effects = attack.GetEffect(this, allies, enemies);
            modifiers.AddRange(effects.UnitModifications);
            spawns.AddRange(effects.UnitSpawns);

        }
        return new BattalionBattleEffects(modifiers, spawns);
    }
}

public enum BattalionAttribute
{
    MaxHitpoints,
    MaxMoral,
    RemainingHitpoints,
    RemainingMoral,
    Defense,
}


public class BattalionEffector
{

    public BattalionBattleEffects GetEffect(BattalionState self, BattleStageSide allies, BattleStageSide enemies)
    {

    }
}

public class BattalionStateModifier
{
    public BattalionEffector Source { get; }

    public BattalionIdentifier Target { get; }
    
    public float HitpointsModification { get; }
}