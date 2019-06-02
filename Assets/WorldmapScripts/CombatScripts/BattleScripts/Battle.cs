using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

public class BattleStateForces
{
    public Army SourceArmy { get; }
    public IEnumerable<SquadBattleState> SquadStates { get; }

    public BattleStateForces(Army sourceArmy, IEnumerable<SquadBattleState> squadStates)
    {
        SourceArmy = sourceArmy;
        SquadStates = squadStates;
    }

    internal BattleStateForces GetNextState(IEnumerable<BattleStateForces> allies, IEnumerable<BattleStateForces> opponents)
    {
        List<SquadBattleState> ret = new List<SquadBattleState>();
        foreach (SquadBattleState ally in SquadStates)
        {
            SquadBattleState nextAlly = ally.GetNextBattleState(allies, opponents);
            ret.Add(nextAlly);
        }
        return new BattleStateForces(SourceArmy, ret);
    }

    internal Army ToArmy()
    {
        IEnumerable<Squad> squads = SquadStates.Select(item => item.ToSquad()).ToArray();
        return new Army(SourceArmy.Leader, SourceArmy.Scouts, SourceArmy.Spies, SourceArmy.Logistics, squads);
    }
}

public class BattleState
{
    public IEnumerable<BattleStateForces> Attackers { get; }
    public IEnumerable<BattleStateForces> Defenders { get; }
    public WarOutcome Outcome { get; }

    public BattleState(IEnumerable<BattleStateForces> attackers, IEnumerable<BattleStateForces> defenders)
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
        if (attackersAlive && !defendersAlive)
        {
            return WarOutcome.AttackersWon;
        }
        if (defendersAlive && !attackersAlive)
        {
            return WarOutcome.DefendersWon;
        }
        return WarOutcome.Draw;
    }

    private static bool GetIsAnyoneRemaining(IEnumerable<BattleStateForces> forces)
    {
        return forces.SelectMany(item => item.SquadStates).Any(item => item.RemainingTroopCount > 0);
    }

    internal BattleState GetNextState()
    {
        IEnumerable<BattleStateForces> nextAttackers = Attackers.Select(item => item.GetNextState(Defenders, Attackers)).ToArray();
        IEnumerable<BattleStateForces> nextDefenders = Defenders.Select(item => item.GetNextState(Attackers, Defenders)).ToArray();
        return new BattleState(nextAttackers, nextDefenders);
    }
}

public class Squad
{
    public SquadDisplayHooks DisplayHooks { get; }
    public int Moral { get; }
    public int Effectiveness { get; }
    public int Discipline { get; }
    public int BaseTacticsGain { get; }
    public int Attack { get; }
    public int Defense { get; }
    public int RankOrder { get; }
    public int TroopCount { get; }
    public int InjuredTroops { get; }
    public int Stealth { get; }
    public int Raiding { get; }
    public int FoodCost { get; }
    public bool IsChaff { get; }

    public Squad(SquadDisplayHooks displayHooks,
        int moral,
        int effectiveness,
        int discipline,
        int baseTacticsGain,
        int attack,
        int defense,
        int rankOrder,
        int troopCount,
        int injuredTroops,
        int stealth,
        int foodCost,
        bool isChaff)
    {
        DisplayHooks = displayHooks;
        Moral = moral;
        Effectiveness = effectiveness;
        Discipline = discipline;
        BaseTacticsGain = baseTacticsGain;
        Attack = attack;
        Defense = defense;
        RankOrder = rankOrder;
        TroopCount = troopCount;
        InjuredTroops = injuredTroops;
        Stealth = stealth;
        FoodCost = foodCost;
        IsChaff = isChaff;
    }
}

public class SquadBattleState
{
    public Squad Source { get; }
    public int EffectiveTactics { get; }
    public int TacticsGainRate { get; }
    public int EffectiveRank { get; }
    public int RemainingTroopCount { get; }
    public int CurrentHitpoints { get; }

    public SquadBattleState(Squad source,
        int effectiveTactics,
        int effectiveRank,
        int tacticsGainRate,
        int remainingTroopCount,
        int currentHitpoints)
    {
        Source = source;
        EffectiveTactics = effectiveTactics;
        EffectiveRank = effectiveRank;
        TacticsGainRate = tacticsGainRate;
        RemainingTroopCount = remainingTroopCount;
        CurrentHitpoints = currentHitpoints;
    }

    internal Squad ToSquad()
    {
        int injury = Source.TroopCount - RemainingTroopCount;
        return new Squad(Source.DisplayHooks,
            Source.Moral,
            Source.Effectiveness,
            Source.Discipline,
            Source.BaseTacticsGain,
            Source.Attack,
            Source.Defense,
            Source.RankOrder,
            Source.TroopCount,
            injury,
            Source.Stealth,
            Source.FoodCost,
            Source.IsChaff);
    }

    internal SquadBattleState GetNextBattleState(IEnumerable<BattleStateForces> allies, IEnumerable<BattleStateForces> opponents)
    {
        int nextTactics = EffectiveTactics + TacticsGainRate;
        int nextRank = GetNextRank(allies);

        int totalDamage = GetTotalDamage(opponents);
        int losses = totalDamage / Source.Defense;
        int nextTroopCount = Math.Max(0, RemainingTroopCount - losses);
        int nextHitpoints = totalDamage % Source.Defense;
        bool routed = GetIsRouted(allies, nextTroopCount);

        return new SquadBattleState(Source,
            nextTactics,
            nextRank,
            TacticsGainRate,
            nextTroopCount,
            nextHitpoints
            );
    }

    private bool GetIsRouted(IEnumerable<BattleStateForces> allies, int nextTroopCount)
    {
        int armyStartingUnits = allies.Sum(item => item.SourceArmy.Squadrons.Where(squad => !squad.IsChaff).Sum(squad => squad.TroopCount));
        int currentUnits = allies.Sum(item => item.SquadStates.Where(squad => !squad.Source.IsChaff).Sum(squad => squad.RemainingTroopCount));
        int squadStartingUnits = Source.TroopCount;

        int armyDamage = 100 - ((currentUnits * 100) / armyStartingUnits);
        int squadDamage = 100 - ((nextTroopCount * 100) / squadStartingUnits);
        int totalMoralDamage = armyDamage + squadDamage;
        return totalMoralDamage > Source.Moral;
    }

    private int GetTotalDamage(IEnumerable<BattleStateForces> opponents)
    {
        int ret = 0;
        foreach (BattleStateForces opponent in opponents)
        {
            foreach (SquadBattleState squad in opponent.SquadStates)
            {
                int damage = squad.GetDamageTo(EffectiveRank);
                ret += damage;
            }
        }
        return ret;
    }

    private int GetDamageTo(int effectiveRank)
    {
        throw new NotImplementedException();
    }

    private int GetNextRank(IEnumerable<BattleStateForces> allies)
    {
        throw new NotImplementedException();
    }
}