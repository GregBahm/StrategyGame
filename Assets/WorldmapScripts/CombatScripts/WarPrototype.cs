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
    public static BattleSite NoBenefit { get; } = new BattleSite(0,0,0);
    public int InitialDefense { get; }
    public int DefenseAdd { get; }
    public int DefenseMultiply { get; }

    public BattleSite(int initialDefense, int defenseAdd, int defenseMultiply)
    {
        InitialDefense = initialDefense;
        DefenseAdd = defenseAdd;
        DefenseMultiply = defenseMultiply;
    }
}

public class WarLoop
{
    public BattleSite Site { get; }

    public WarStageSetup InitialState { get; }

    public NoncombatWarPhase PreBattle { get; }

    public Battle Battle { get; }
    
    public WarOutcome Outcome { get; }

    public WarLoop(WarStageSetup initialState, BattleSite site)
    {
        InitialState = initialState;
        Site = site;
        PreBattle = new NoncombatWarPhase(initialState.Attackers, initialState.Defenders, site);
        Battle = GetBattle(initialState);
        Outcome = Battle.Outcome;
    }

    private Battle GetBattle(WarStageSetup setup)
    {
        IEnumerable<ArmyInBattle> attackers = GetBattleArmies(setup.Attackers, PreBattle.AttackerEffects, BattleSite.NoBenefit).ToArray();
        IEnumerable<ArmyInBattle> defenders = GetBattleArmies(setup.Defenders, PreBattle.DefenderEffects, Site).ToArray();
        BattleState initialState = new BattleState(attackers, defenders);
        return new Battle(initialState);
    }

    private IEnumerable<ArmyInBattle> GetBattleArmies(WarForces forces, NoncombatWarEffects preBattle, BattleSite site)
    {
        foreach (Army army in forces.Armies)
        {
            yield return GetArmyInBattle(army, preBattle, site);
        }
    }

    private ArmyInBattle GetArmyInBattle(Army army, NoncombatWarEffects preBattle, BattleSite site)
    {
        IEnumerable<SquadInBattle> squads = army.Squadrons.Select(item => GetSquadInBattle(item, army, preBattle, site));

    }

    private SquadInBattle GetSquadInBattle(Squad squad, Army army, NoncombatWarEffects preBattle, BattleSite site)
    {

    }
}

public class NoncombatWarEffects
{
    public ScoutingEffects Scouting { get; }
    public RaidingEffects Raiding { get; }
    public LogisticalEffects Logistic { get; }
    public SpyingEffects Spying { get; }

    public NoncombatWarEffects(ScoutingEffects scouting, RaidingEffects raiding, LogisticalEffects logistic, SpyingEffects spying)
    {
        Scouting = scouting;
        Raiding = raiding;
        Logistic = logistic;
        Spying = spying;
    }
}

public class ScoutingEffects
{
    public int ScoutBonus { get; }
    public int ScoutsSum { get; }
    public int Scouted { get; }
    
    public ScoutingEffects(WarForces self, WarForces other)
    {
        ScoutsSum = self.Armies.Sum(item => item.Scouts.ScoutingEffectiveness);
        Scouted = other.Armies.SelectMany(item => item.Squadrons).Sum(item => GetScoutingVisibility(item));
        ScoutBonus = ScoutsSum * Scouted;
    }

    private int GetScoutingVisibility(Squad item)
    {
        return item.TroopCount * item.Stealth;
    }
}

public class RaidingEffects
{
    public static RaidingEffects Defender { get; } = new RaidingEffects();

    public int RaidingSum { get; }
    public int LogisticsDrain { get; }
    
    private RaidingEffects()
    { }
    public RaidingEffects(WarForces attacker, BattleSite site)
    {
        RaidingSum = attacker.Armies.SelectMany(item => item.Squadrons).Sum(item => item.Raiding);
        LogisticsDrain = Math.Max(0, RaidingSum - site.InitialDefense);
    }
}

public class LogisticalEffects
{
    public int FoodNeeds { get; }
    public int SumFood { get; }
    public int SumMedical { get; }
    public int SumEquipment { get; }

    public LogisticalEffects(WarForces forces)
    {
        FoodNeeds = forces.Armies.SelectMany(item => item.Squadrons).Sum(item => item.FoodCost);
        SumFood = forces.Armies.Sum(item => item.Logistics.Food);
        SumMedical = forces.Armies.Sum(item => item.Logistics.Medical);
        SumEquipment = forces.Armies.Sum(item => item.Logistics.Equipment);
    }
}

public class SpyingEffects
{
    public int SpySum { get; }
    public int Sabotage { get; }

    public SpyingEffects(WarForces forces)
    {
        SpySum = forces.Armies.Sum(item => item.Spies.LeaderDrain);
        Sabotage = forces.Armies.Sum(item => item.Spies.SupplySabotage);
    }
}


public class NoncombatWarPhase
{
    public NoncombatWarEffects AttackerEffects { get; }
    public NoncombatWarEffects DefenderEffects { get; }

    public NoncombatWarPhase(WarForces attacker, WarForces defender, BattleSite site)
    {
        ScoutingEffects attackerScout = new ScoutingEffects(attacker, defender);
        ScoutingEffects defenderScout = new ScoutingEffects(defender, attacker);
        SpyingEffects attackerSpies = new SpyingEffects(attacker);
        SpyingEffects defenderSpies = new SpyingEffects(defender);
        RaidingEffects raiding = new RaidingEffects(attacker, site);
        LogisticalEffects attackerLogistics = new LogisticalEffects(attacker);
        LogisticalEffects defenderLogistics = new LogisticalEffects(defender);

        AttackerEffects = new NoncombatWarEffects(attackerScout, raiding, attackerLogistics, attackerSpies);
        DefenderEffects = new NoncombatWarEffects(defenderScout, RaidingEffects.Defender, defenderLogistics, defenderSpies);
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
    public int Stealth { get; }
    public int Raiding { get; }
    public int FoodCost { get; }

    public Squad(SquadDisplayHooks squadDisplayHooks, 
        int moral, 
        int effectiveness, 
        int discipline, 
        int baseTacticsGain, 
        int attack, 
        int defense, 
        int rankOrder, 
        IEnumerable<ThreatRange> threatRange, 
        int troopCount,
        int injuredTroops,
        int stealth,
        int foraging)
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
        Stealth = stealth;
        FoodCost = foraging;
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