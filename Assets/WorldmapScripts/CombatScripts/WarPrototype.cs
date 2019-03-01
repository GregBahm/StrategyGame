using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class WarPrototype : MonoBehaviour
{
}

public class BattleSite
{
    public float InitialDefense { get; }
    public float DefenseAdd { get; }
    public float DefenseMultiply { get; }
}

public class BattleLoop
{
    public BattleSite Site { get; }

    public WarStageSetup InitialState { get; }

    public WarStageSetup NextLoopState { get; }

    public BattleOutcome Outcome { get; }

    public BattleLoop(WarStageSetup initialState, BattleSite site)
    {
        InitialState = initialState;
        Site = site;
        
    }
}

public class WarStageSetup
{
    public WarForces Attackers { get; }
    public WarForces Defeners { get; }

    public WarStageSetup(WarForces attackers, WarForces defenders)
    {
        Attackers = attackers;
        Defeners = defenders;
    }
}

public class Squad
{
    public SquadDisplayHooks SquadDisplayHooks { get; }
    public int Moral { get; }
    public int Effectiveness { get; }
    public int Discipline { get; }
    public int BaseTacticsGain { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int Rank { get; }
    public IEnumerable<ThreatRange> ThreatRange { get; }
    public int TroopCount { get; }


}

public class ThreatRange
{
    public int MinOffset { get; }
    public int MaxOffset { get; }
    public int MinSpan { get; }
    public int MaxSpan { get; }

    public ThreatRange(int minOffset, int maxOffset, int minSpan, int maxSpan)
    {
        MinOffset = minOffset;
        MaxOffset = maxOffset;
        MinSpan = minSpan;
        MaxSpan = maxSpan;
    }
}


public class Leader
{
    public LeaderDisplayHooks DisplayHooks { get; }
    public int Fundamentals { get; }
    public int Cleverness { get; }
    public Leader(LeaderDisplayHooks displayHooks, int fundamentals, int cleverness)
    {
        DisplayHooks = displayHooks;
        Cleverness = cleverness;
        Fundamentals = fundamentals;
    }
}

public class Scouts
{
    public ScoutDisplayHooks DisplayHooks { get; }
    public int ScoutingEffectiveness { get; }

    public Scouts(ScoutDisplayHooks displayHooks, int scoutingEffectiveness)
    {
        DisplayHooks = displayHooks;
        ScoutingEffectiveness = scoutingEffectiveness;
    }
}

public class Army
{
    public Leader Leader { get; }
    public Scouts Scouts { get; }
    public Spies Spies { get; }
    public Logistics Logistics { get; }
    public IEnumerable<Squad> Squadrons { get; }
}

public class Spies
{
    public SpiesDisplayHooks DisplayHooks { get; }
    public int LeaderDrain { get; }
    public int SupplySabotage { get; }

    public Spies(SpiesDisplayHooks displayHooks, int leaderDrain, int supplySabotage)
    {
        DisplayHooks = displayHooks;
        LeaderDrain = leaderDrain;
        SupplySabotage = supplySabotage;
    }
}

public class Logistics
{
    public LogisticsDisplayHooks DisplayHooks { get; }
    public int Food { get; }
    public int Equipment { get; }
    public int Medical { get; }

    public Logistics(LogisticsDisplayHooks displayHooks, int food, int equipment, int medical)
    {
        DisplayHooks = displayHooks;
        Food = food;
        Equipment = equipment;
        Medical = medical;
    }
}

public class SquadDisplayHooks { }
public class LogisticsDisplayHooks { }
public class SpiesDisplayHooks { }
public class LeaderDisplayHooks { }
public class ScoutDisplayHooks { }