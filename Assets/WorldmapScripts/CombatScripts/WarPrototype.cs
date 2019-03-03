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
        NoncombatWarPhase attackerPreBattle = new NoncombatWarPhase(setup.Attackers, setup.Defenders);
        NoncombatWarPhase defenderPreBattle = new NoncombatWarPhase(setup.Defenders, setup.Attackers);
        IEnumerable<ArmyInBattle> attackers = GetBattleArmies(setup.Attackers, attackerPreBattle).ToArray();
        IEnumerable<ArmyInBattle> defenders = GetBattleArmies(setup.Defenders, defenderPreBattle).ToArray();
        BattleState initialState = new BattleState(attackers, defenders);
        return new Battle(initialState);
    }

    private IEnumerable<ArmyInBattle> GetBattleArmies(WarForces forces, NoncombatWarPhase preBattle)
    {
        foreach (Army army in forces.Armies)
        {
            foreach (Squad squad in army.Squadrons)
            {

            }
        }
    }
}

public class NoncombatWarPhase
{
    public NoncombatWarPhase(WarForces self, WarForces enemy)
    {
        // Scouts
        //  - Input: + Sum of scouts in war forces - Each unit * Unit Stealthiness
        //  - Output: flat tactical bonus at start of battle, similar to defense

        // Static Defenses
        //  - Input: Site
        //  - Output: flat tactical bonus at start of battle for defenders

        // Spying
        //  - Input: + Friendly spies vs enemy spies
        //  - Output: Drains leadership attributes from one leader to another

        // Sabotage
        //  - Input: Spy ratio and sum sabotage factor 
        //  - Output: Reduces enemy logistics by sabotage * spy ratio

        // Raiding
        //  - Input: Sum raiding factor in troops - site defenses
        //  - Output: 

        // Logistic Effectiveness (Food) 
        //  - Input: Sum food logistics + foraging - sum troops troops
        //  - Output: Combat effectiveness

        // Logistic Effectiveness (Medical)
        //  - Input: Sum medical logistics
        //  - Output: Injured troops converted back to troops

        // Logistic Effectiveness (Equipment)
        //  - Input: Sum equipment logistics
        //  - Output: Combat effectiveness
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
    public int InjuredTroops { get; }

    public Squad(SquadDisplayHooks squadDisplayHooks, 
        int moral, 
        int effectiveness, 
        int discipline, 
        int baseTacticsGain, 
        int attack, 
        int defense, 
        int rankOrder, 
        IEnumerable<ThreatRange> threatRange, 
        int troopCount
        int injuredTroops)
    {
        SquadDisplayHooks = squadDisplayHooks;
        Moral = moral;
        Effectiveness = effectiveness;
        Discipline = discipline;
        BaseTacticsGain = baseTacticsGain;
        Attack = attack;
        Defense = defense;
        RankOrder = rankOrder;
        ThreatRange = threatRange;
        TroopCount = troopCount;
        InjuredTroops = injuredTroops;
    }
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

    public SquadBattleState(int effectiveAttack, 
        int effectiveDefense, 
        int remainingMoral, 
        int effectiveTactics, 
        int effectiveRank, 
        IEnumerable<ThreatRangeState> effectiveThreatRange, 
        int remainingTroopCount, 
        int currentHitpoints)
    {
        EffectiveAttack = effectiveAttack;
        EffectiveDefense = effectiveDefense;
        RemainingMoral = remainingMoral;
        EffectiveTactics = effectiveTactics;
        EffectiveRank = effectiveRank;
        EffectiveThreatRange = effectiveThreatRange;
        RemainingTroopCount = remainingTroopCount;
        CurrentHitpoints = currentHitpoints;
    }
}

public class BattleState
{
    public IEnumerable<ArmyInBattle> Attackers { get; }
    public IEnumerable<ArmyInBattle> Defenders { get; }
    public WarOutcome Outcome { get; }

    public BattleState(IEnumerable<ArmyInBattle> attackers, IEnumerable<ArmyInBattle> defenders)
    {
        Attackers = attackers;
        Defenders = defenders;
        Outcome = GetOutcome();
    }

    private WarOutcome GetOutcome()
    {
        bool attackersAlive = GetIsAnyoneRemaining(Attackers);
        bool defendersAlive = GetIsAnyoneRemaining(Defenders);
        if (attackersAlive && defendersAlive)
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

    private static bool GetIsAnyoneRemaining(IEnumerable<ArmyInBattle> forces)
    {
        return forces.SelectMany(item => item.Squadrons).Any(item => item.State.RemainingTroopCount > 0);
    }

    internal BattleState GetNextState()
    {
        IEnumerable<ArmyInBattle> nextAttackers = Attackers.Select(item => item.GetNextState(Defenders)).ToArray();
        IEnumerable<ArmyInBattle> nextDefenders = Defenders.Select(item => item.GetNextState(Attackers)).ToArray();
        return new BattleState(nextAttackers, nextDefenders);
    }

    internal WarStageSetup GetNextWarStageSetup(Faction attackingFaction, Faction defendingFaction)
    {
        WarForces attackers = ToWarForces(attackingFaction, Attackers);
        WarForces defenders = ToWarForces(defendingFaction, Defenders);
        return new WarStageSetup(attackers, defenders);
    }

    private WarForces ToWarForces(Faction faction, IEnumerable<ArmyInBattle> attackers)
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

public class SquadInBattle
{
    public Squad Squad { get; }
    public SquadBattleState State { get; }

    public SquadInBattle(Squad squad, SquadBattleState state)
    {
        Squad = squad;
        State = state;
    }
}
public class ArmyInBattle
{
    public Army ArmyAtWarStart { get; }
    public Army ArmyAtBattleStart { get; }
    public IEnumerable<SquadInBattle> Squadrons { get; }

    public ArmyInBattle(Army armyAtWarStart, Army armyAtBattleStart)
    {
        ArmyAtWarStart = armyAtWarStart;
        ArmyAtBattleStart = armyAtBattleStart;
        Squadrons = CreateInitialSquadrons().ToArray();
    }

    public ArmyInBattle(Army armyAtWarStart, Army armyAtBattleStart, IEnumerable<SquadInBattle> squadrons)
    {
        ArmyAtWarStart = armyAtWarStart;
        ArmyAtBattleStart = armyAtBattleStart;
        Squadrons = squadrons;
    }

    private IEnumerable<SquadInBattle> CreateInitialSquadrons()
    {
        foreach (Squad squad in ArmyAtBattleStart.Squadrons)
        {
            SquadBattleState state = GetSquadBattleState(squad, ArmyAtBattleStart);
            yield return new SquadInBattle(squad, state);
        }
    }

    private SquadBattleState GetSquadBattleState(Squad squad, Army armyAtBattleStart)
    {
        throw new NotImplementedException();
    }

    internal ArmyInBattle GetNextState(IEnumerable<ArmyInBattle> defenders)
    {
        IEnumerable<SquadInBattle> newArmy = GetNextArmyState(defenders);
        return new ArmyInBattle(ArmyAtWarStart, ArmyAtBattleStart, newArmy);
    }

    private IEnumerable<SquadInBattle> GetNextArmyState(IEnumerable<ArmyInBattle> defenders)
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