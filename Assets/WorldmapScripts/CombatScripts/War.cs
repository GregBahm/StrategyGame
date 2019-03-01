using UnityEngine;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System;
using System.Linq;

public class War
{
    public Faction Winner { get; }
    public Province Location { get; }

    public ReadOnlyCollection<BattleLoop> Progression { get; }

    public War(ProvinceState location, WarForces attackers, WarForces defenders)
    {
        Location = location.Identifier;
        IEnumerable<BattleSite> sites = GetSites(location);
        WarStageSetup initialState = new WarStageSetup(attackers, defenders);
        Progression = GetProgression(initialState, sites);
        Winner = GetWinner(attackers.Faction, defenders.Faction);
    }

    private Faction GetWinner(Faction attackers, Faction defenders)
    {
        if(Progression.Last().Outcome == BattleOutcome.AttackersWon)
        {
            return attackers;
        }
        else
        {
            return defenders;
        }
    }

    private ReadOnlyCollection<BattleLoop> GetProgression(WarStageSetup currentState, IEnumerable<BattleSite> sites)
    {
        List<BattleLoop> ret = new List<BattleLoop>();
        foreach (BattleSite site in sites)
        {
            BattleLoop loop = new BattleLoop(currentState, site);
            ret.Add(loop);
            if (loop.Outcome != BattleOutcome.AttackersWon)
            {
                break;
            }
            currentState = loop.NextLoopState;
        }
        return ret.AsReadOnly();
    }

    private IEnumerable<BattleSite> GetSites(ProvinceState defendedProvince)
    {
        throw new NotImplementedException();
    }
}
