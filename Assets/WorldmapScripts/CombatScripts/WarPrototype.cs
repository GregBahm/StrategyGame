using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;


public class War
{
    public Faction Winner { get; }
    public Province Location { get; }

    public ReadOnlyCollection<CampaignProgression> Progression { get; }

    public War(ProvinceState location, FactionArmies attackers, FactionArmies defenders)
    {
        Location = location.Identifier;
        IEnumerable<BattleSite> sites = GetSites(location);
        Progression = GetProgression(attackers, defenders, sites);
        Winner = GetWinner(attackers.Faction, defenders.Faction);
    }

    private Faction GetWinner(Faction attackers, Faction defenders)
    {
        if (Progression.Last().Outcome == WarOutcome.AttackersWon)
        {
            return attackers;
        }
        else
        {
            return defenders;
        }
    }

    private ReadOnlyCollection<CampaignProgression> GetProgression(FactionArmies attackers, FactionArmies defenders, IEnumerable<BattleSite> sites)
    {
        List<CampaignProgression> ret = new List<CampaignProgression>();
        CampaignProgression loop = new CampaignProgression(attackers, defenders, sites);
        foreach (BattleSite site in sites)
        {
            ret.Add(loop);
            if (loop.Outcome != WarOutcome.AttackersWon)
            {
                break;
            }
            loop = loop.GetNext();
        }
        return ret.AsReadOnly();
    }

    private IEnumerable<BattleSite> GetSites(ProvinceState defendedProvince)
    {
        throw new NotImplementedException();
    }
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

public class CampaignProgression
{
    public BattleSite CurrentSite { get; }
    public IEnumerable<BattleSite> RemainingSites { get; }

    public FactionArmies Attackers { get; }
    public FactionArmies Defenders { get; }

    public BattlePreparation PreBattle { get; }

    public Battle Battle { get; }
    
    public WarOutcome Outcome { get; }

    public CampaignProgression(FactionArmies attackers, FactionArmies defenders, IEnumerable<BattleSite> sites)
    {
        Attackers = attackers;
        Defenders = defenders;
        RemainingSites = sites;
        CurrentSite = sites.First();
        PreBattle = new BattlePreparation(Attackers, Defenders, CurrentSite);
        Battle = GetBattle();
        Outcome = Battle.Outcome;
    }

    private Battle GetBattle()
    {
        IEnumerable<BattleStateForces> attackers = GetInitialBattleArmies(Attackers, PreBattle.Attacker, BattleSite.NoBenefit).ToArray();
        IEnumerable<BattleStateForces> defenders = GetInitialBattleArmies(Defenders, PreBattle.Defender, CurrentSite).ToArray();
        BattleState initialState = new BattleState(attackers, defenders);
        return new Battle(initialState);
    }

    private IEnumerable<BattleStateForces> GetInitialBattleArmies(FactionArmies forces, BattleSidePreparation preBattle, BattleSite site)
    {
        foreach (Army army in forces.Armies)
        {
            yield return GetInitialArmyInBattle(army, preBattle, site);
        }
    }

    private BattleStateForces GetInitialArmyInBattle(Army army, BattleSidePreparation preBattle, BattleSite site)
    {
        IEnumerable<SquadBattleState> squads = army.Squadrons.Select(item => GetInitialSquadInBattle(item, army, preBattle, site)).ToArray();
        return new BattleStateForces(army, squads);

    }

    private SquadBattleState GetInitialSquadInBattle(Squad item, Army army, BattleSidePreparation preBattle, BattleSite site)
    {
        throw new NotImplementedException();
    }

    public CampaignProgression GetNext()
    {
        BattleState lastBattleState = Battle.States.Last();
        IEnumerable<BattleSite> newRemainingSites = RemainingSites.Skip(1).ToArray();
        FactionArmies newAttackers = ToFactionArmies(lastBattleState.Attackers, Attackers.Faction);
        FactionArmies newDefenders = ToFactionArmies(lastBattleState.Defenders, Defenders.Faction);
        return new CampaignProgression(newAttackers, newDefenders, newRemainingSites);
    }

    private FactionArmies ToFactionArmies(IEnumerable<BattleStateForces> armiesInBattle, Faction faction)
    {
        IEnumerable<Army> armies = armiesInBattle.Select(item => item.ToArmy()).ToArray();
        return new FactionArmies(faction, armies);
    }
}

public class FactionArmies
{
    public Faction Faction { get; }
    public IEnumerable<Army> Armies { get; }

    public FactionArmies(Faction faction,
        IEnumerable<Army> armies)
    {
        Faction = faction;
        Armies = armies;
    }

    internal static FactionArmies CombineForces(IEnumerable<FactionArmies> forces, Faction faction)
    {
        IEnumerable<Army> armies = forces.SelectMany(item => item.Armies).ToArray();
        return new FactionArmies(faction, armies);
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

public class Army
{
    public Leader Leader { get; }
    public Scouts Scouts { get; }
    public Spies Spies { get; }
    public Logistics Logistics { get; }
    public IEnumerable<Squad> Squadrons { get; }

    public Army(Leader leader, Scouts scouts, Spies spies, Logistics logistics, IEnumerable<Squad> squadrons)
    {
        Leader = leader;
        Scouts = scouts;
        Spies = spies;
        Logistics = logistics;
        Squadrons = squadrons;
    }
}