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
    public BattleLoopState InitialState { get; }
    public BattleLoopState AfterScouting { get; }
    public BattleLoopState AfterSpying { get; }
    public BattleLoopState AfterSupplying { get; }
    public BattleLoopState AfterBattle { get; }
    public BattleOutcome Outcome { get; }

    public BattleLoop(BattleLoopState initialState, BattleSite site)
    {
        InitialState = initialState;
        Site = site;
        AfterScouting = GetAfterScouting();
        AfterSpying = GetAfterSpying();
        AfterSupplying = GetAfterSupplying();
        AfterBattle = GetAfterBattle();
    }

    private BattleLoopState GetAfterScouting()
    {
        throw new NotImplementedException();
    }

    private BattleLoopState GetAfterSpying()
    {
        throw new NotImplementedException();
    }

    private BattleLoopState GetAfterSupplying()
    {
        throw new NotImplementedException();
    }

    private BattleLoopState GetAfterBattle()
    {
        throw new NotImplementedException();
    }
}

public class BattleLoopState
{
    public WarForces Attackers { get; }
    public WarForces Defeners { get; }

    public BattleLoopState(WarForces attackers, WarForces defenders)
    {
        Attackers = attackers;
        Defeners = defenders;
    }
}


public class TroopBlueprint
{

}

public class Squad
{

}

public class LeaderBlueprint { }

public class Leader { }

public class ScoutBlueprint { }

public class Army
{
    public Leader Leader { get; }
    public IEnumerable<Squad> Squadrons { get; }
}

public class Scouts { }
public class Spies { }
public class Supplies { }