using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class WarsResolver
{
    public IEnumerable<War> Wars { get; }
    public GameState NewGameState { get; }

    public WarsResolver(GameState state, IEnumerable<AttackMove> attackMoves)
    {
        WarForcesTable warForcesTable = new WarForcesTable(state, attackMoves);
        WarSorter warSorter = new WarSorter(state, warForcesTable, attackMoves);
        
        Wars = warSorter.FinalWars.SelectMany(item => item.Wars).ToArray();

        NewGameState = GetNewGameState(state, Wars);
    }

    private static GameState GetNewGameState(GameState state, IEnumerable<War> wars)
    {
        Dictionary<Province, ProvinceState> oldNewDictionary = state.Provinces.ToDictionary(item => item.Identifier, item => item);
        foreach (War war in wars)
        {
            ProvinceState oldState = oldNewDictionary[war.Location];
            ProvinceState newState = new ProvinceState(war.Winner, oldState.Upgrades, oldState.Identifier, oldState.Tiles);
            oldNewDictionary[war.Location] = newState;
        }
        return new GameState(oldNewDictionary.Values);
    }

    private class WarSorter
    {
        public IEnumerable<FinalWarsSetup> FinalWars { get; }

        public WarSorter(GameState state, WarForcesTable forcesAllocator, IEnumerable<AttackMove> attackMoves)
        {
            Dictionary<Province, List<AttackMove>> byTargetTable = SortByTarget(attackMoves);
            IEnumerable<FinalWarsSetup> finalWars = GetFinalWars(state, byTargetTable, forcesAllocator).ToArray();
            FinalWars = finalWars.ToArray();
        }

        private IEnumerable<FinalWarsSetup> GetFinalWars(GameState state, Dictionary<Province, List<AttackMove>> byTargetTable, WarForcesTable forcesTable)
        {
            foreach (KeyValuePair<Province, List<AttackMove>> item in byTargetTable)
            {
                FactionArmies defender = forcesTable.GetForcesFor(item.Key);
                IEnumerable<FactionArmies> invaders = GetAttackersForProvince(item.Key, item.Value, forcesTable).ToArray();
                ProvinceState locationState = state.GetProvinceState(item.Key);
                FinalWarsSetup retItem = new FinalWarsSetup(locationState, defender, invaders);
                yield return retItem;
            }
        }

        private IEnumerable<FactionArmies> GetAttackersForProvince(Province defender, IEnumerable<AttackMove> invaders, WarForcesTable forcesTable)
        {
            IEnumerable<IEnumerable<AttackMove>> invadersByFaction = GetInvadersByFaction(invaders).ToArray();
            foreach (IEnumerable<AttackMove> invadingFaction in invadersByFaction)
            {
                FactionArmies retItem = forcesTable.GetForcesFor(invadingFaction);
                yield return retItem;
            }
        }

        private IEnumerable<IEnumerable<AttackMove>> GetInvadersByFaction(IEnumerable<AttackMove> invaders)
        {
            Dictionary<Faction, List<AttackMove>> ret = new Dictionary<Faction, List<AttackMove>>();
            foreach (AttackMove item in invaders)
            {
                if (ret.ContainsKey(item.Faction))
                {
                    ret[item.Faction].Add(item);
                }
                else
                {
                    ret.Add(item.Faction, new List<AttackMove>() { item });
                }
            }
            return ret.Values;
        }

        private Dictionary<Province, List<AttackMove>> SortByTarget(IEnumerable<AttackMove> attacks)
        {
            Dictionary<Province, List<AttackMove>> ret = new Dictionary<Province, List<AttackMove>>();
            foreach (AttackMove attack in attacks)
            {
                if(ret.ContainsKey(attack.TargetProvince))
                {
                    ret[attack.TargetProvince].Add(attack);
                }
                else
                {
                    List<AttackMove> newValue = new List<AttackMove> { attack };
                    ret.Add(attack.TargetProvince, newValue);
                }
            }
            return ret;
        }
    }

    private class FinalWarsSetup
    {
        public IEnumerable<War> Wars { get; }
        public Faction FinalWinner { get; } // Null if multiple invaders beat the defender

        public FinalWarsSetup(ProvinceState location, FactionArmies defender, IEnumerable<FactionArmies> invaders)
        {
            Wars = GetWars(location, defender, invaders);
            FinalWinner = GetFinalWindner(defender.Faction, Wars);
        }

        private static Faction GetFinalWindner(Faction defendingFaction, IEnumerable<War> wars)
        {
            int victoriousInvaders = wars.Count(item => item.Winner != defendingFaction);
            if (victoriousInvaders == 0)
            {
                return defendingFaction;
            }
            if(victoriousInvaders == 1)
            {
                return wars.First(item => item.Winner != defendingFaction).Winner;
            }
            return null;
        }

        private static IEnumerable<War> GetWars(ProvinceState location, FactionArmies defender, IEnumerable<FactionArmies> invaders)
        {
            List<War> ret = new List<War>();
            foreach (FactionArmies invader in invaders)
            {
                War war = new War(location, invader, defender);
                ret.Add(war);
            }
            return ret;
        }
    }

    private class WarForcesTable
    {
        private ReadOnlyDictionary<AttackMove, FactionArmies> _attackerForces;

        private ReadOnlyDictionary<Province, FactionArmies> _defenderForces;

        public WarForcesTable(GameState state, IEnumerable<AttackMove> attackMoves)
        {
            _attackerForces = GetAttackerForces(state, attackMoves);
            _defenderForces = GetDefenderForces(state, attackMoves);
        }

        private FactionArmies CalculateDefendingForces(ProvinceState target, int attacksByDefender, GameState state)
        {
            // TODO: Calculate defending forces using these factors
            return new FactionArmies(target.Owner, new Army[0]);
        }

        private FactionArmies CalculateAttackingForces(AttackMove move, int attacksByFaction, GameState state)
        {
            // TODO: Calculate war forces using these factors
            return new FactionArmies(move.Faction, new Army[0]);
        }

        private ReadOnlyDictionary<Province, FactionArmies> GetDefenderForces(GameState state, IEnumerable<AttackMove> attacks)
        {
            Dictionary<Province, FactionArmies> ret = new Dictionary<Province, FactionArmies>();
            HashSet<Province> targets = new HashSet<Province>(attacks.Select(item => item.TargetProvince));
            foreach (Province target in targets)
            {
                ProvinceState provinceOwner = state.GetProvinceState(target);
                int attacksByDefender = attacks.Count(item => item.Faction == provinceOwner.Owner);
                FactionArmies forces = CalculateDefendingForces(provinceOwner, attacksByDefender, state);
                ret.Add(target, forces);
            }
            return new ReadOnlyDictionary<Province, FactionArmies>(ret);
        }

        private ReadOnlyDictionary<AttackMove, FactionArmies> GetAttackerForces(GameState state, IEnumerable<AttackMove> attackMoves)
        {
            Dictionary<AttackMove, FactionArmies> ret = new Dictionary<AttackMove, FactionArmies>();
            foreach (AttackMove move in attackMoves)
            {
                int attacksByFaction = attackMoves.Count(item => item.Faction == move.Faction);
                FactionArmies forces = CalculateAttackingForces(move, attacksByFaction, state);
                ret.Add(move, forces);
            }
            return new ReadOnlyDictionary<AttackMove, FactionArmies>(ret);
        }

        public FactionArmies GetForcesFor(AttackMove attackMove)
        {
            return _attackerForces[attackMove];
        }
        public FactionArmies GetForcesFor(IEnumerable<AttackMove> convergingAttacks)
        {
            IEnumerable<FactionArmies> forces = convergingAttacks.Select(item => _attackerForces[item]);
            return FactionArmies.CombineForces(forces, convergingAttacks.First().Faction);
        }

        public FactionArmies GetForcesFor(Province defender)
        {
            return _defenderForces[defender];
        }
    }
}
