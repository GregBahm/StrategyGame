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

    public ReadOnlyCollection<WarLoop> Progression { get; }

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
        if(Progression.Last().Outcome == WarOutcome.AttackersWon)
        {
            return attackers;
        }
        else
        {
            return defenders;
        }
    }

    private ReadOnlyCollection<WarLoop> GetProgression(WarStageSetup currentState, IEnumerable<BattleSite> sites)
    {
        List<WarLoop> ret = new List<WarLoop>();
        foreach (BattleSite site in sites)
        {
            WarLoop loop = new WarLoop(currentState, site);
            ret.Add(loop);
            if (loop.Outcome != WarOutcome.AttackersWon)
            {
                break;
            }
            currentState = loop.Battle.States.Last().GetNextWarStageSetup(currentState.Attackers.Faction, currentState.Defenders.Faction);
        }
        return ret.AsReadOnly();
    }
    
    private IEnumerable<BattleSite> GetSites(ProvinceState defendedProvince)
    {
        throw new NotImplementedException();
    }
}
