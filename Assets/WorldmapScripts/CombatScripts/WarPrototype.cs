using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

public class WarLoop
{
    public BattleSite Site { get; }

    public WarStageSetup InitialState { get; }

    public Battle Battle { get; }
    
    public WarOutcome Outcome { get; }

    public WarLoop(WarStageSetup initialState, BattleSite site)
    {
        InitialState = initialState;
        Battle = GetBattle(initialState);
        Outcome = Battle.Outcome;
    }

    private Battle GetBattle(WarStageSetup setup)
    {
        Prewar attackerPrewar = new Prewar(setup.Attackers, setup.Defenders);
        IEnumerable<SquadBattleBinding> attackers = setup.Attackers.Select(item => new SquadBattleBinding())
        IEnumerable<SquadBattleBinding> defenders;
        BattleState initialState = new BattleState(attackers, defenders);
        return new Battle(initialState);
    }
}

public class Prewar
{
    public Prewar()
    {

    }
}

public class WarStageSetup
{
    public WarForces Attackers { get; }
    public WarForces Defenders { get; }

    public WarStageSetup(WarForces attackers, WarForces defenders)
    {
        Attackers = attackers;
        Defenders = defenders;
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
    public int RankOrder { get; }
    public IEnumerable<ThreatRange> ThreatRange { get; }
    public int TroopCount { get; }
}

public class SquadBattleState
{
    public int EffectiveAttack { get; }
    public int EffectiveDefense { get; }
    public int RemainingMoral { get; }
    public int EffectiveTactics { get; }
    public int EffectiveRank { get; }
    public IEnumerable<ThreatRangeState> EffectiveThreatRange { get; }
    public int RemainingTroopCount { get; }
    public int CurrentHitpoints { get; }
}

public class BattleState
{
    public IEnumerable<SquadBattleBinding> Attackers { get; }
    public IEnumerable<SquadBattleBinding> Defenders { get; }
    public WarOutcome Outcome { get; }

    public BattleState(IEnumerable<SquadBattleBinding> attackers, IEnumerable<SquadBattleBinding> defenders)
    {
        Attackers = attackers;
        Defenders = defenders;
        Outcome = GetOutcome();
    }

    private WarOutcome GetOutcome()
    {
        bool attackersAlive = Attackers.Any(item => item.State.RemainingTroopCount > 0);
        bool defendersAlive = Defenders.Any(item => item.State.RemainingTroopCount > 0);
        if(attackersAlive && defendersAlive)
        {
            return WarOutcome.Ongoing;
        }
        if(attackersAlive && !defendersAlive)
        {
            return WarOutcome.AttackersWon;
        }
        if(defendersAlive && !attackersAlive)
        {
            return WarOutcome.DefendersWon;
        }
        return WarOutcome.Draw;
    }

    internal BattleState GetNextState()
    {
        IEnumerable<SquadBattleBinding> nextAttackers = Attackers.Select(item => item.GetNextState(Defenders)).ToArray();
        IEnumerable<SquadBattleBinding> nextDefenders = Defenders.Select(item => item.GetNextState(Attackers)).ToArray();
        return new BattleState(nextAttackers, nextDefenders);
    }

    internal WarStageSetup GetNextWarStageSetup(Faction attackingFaction, Faction defendingFaction)
    {
        WarForces attackers = ToWarForces(attackingFaction, Attackers);
        WarForces defenders = ToWarForces(defendingFaction, Defenders);
        return new WarStageSetup(attackers, defenders);
    }

    private WarForces ToWarForces(Faction faction, IEnumerable<SquadBattleBinding> attackers)
    {
        IEnumerable<Army> newArmies = attackers.Select(item => item.ToArmy()).ToArray();
        return new WarForces(faction, newArmies);
    }
}

public class Battle
{
    public const int BattleTurnLimit = 10000;
    public ReadOnlyCollection<BattleState> States { get; }
    public WarOutcome Outcome { get; }

    public Battle(BattleState startingState)
    {
        List<BattleState> states = new List<BattleState>();
        BattleState currentState = startingState;
        while(currentState.Outcome == WarOutcome.Ongoing && states.Count < BattleTurnLimit)
        {
            states.Add(currentState);
            currentState = currentState.GetNextState();
        }
        States = states.AsReadOnly();
        Outcome = states.Count < BattleTurnLimit ? currentState.Outcome : WarOutcome.Draw;
    }
}


public class SquadBattleBinding
{
    public Squad SquadBasis { get; }
    public Leader SquadLeader { get; }
    public SquadBattleState State { get; }

    internal SquadBattleBinding GetNextState(IEnumerable<SquadBattleBinding> defenders)
    {
        throw new NotImplementedException();
    }

    internal Army ToArmy()
    {
        throw new NotImplementedException();
    }
}


public class ThreatRangeState
{
    public int Offset { get; }
    public int Span { get; }

    public ThreatRangeState(int offset, int span)
    {
        Offset = offset;
        Span = span;
    }
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