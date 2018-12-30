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
        WarSorter warSorter = new WarSorter(warForcesTable, attackMoves);
        
        Wars = warSorter.FinalWars.SelectMany(item => item.Wars).ToArray();

        NewGameState = GetNewGameState(state, Wars);
    }

    private static GameState GetNewGameState(GameState state, IEnumerable<War> wars)
    {
        Dictionary<Province, ProvinceState> oldNewDictionary = state.Provinces.ToDictionary(item => item.Identifier, item => item);
        foreach (War war in wars)
        {
            ProvinceState oldState = oldNewDictionary[war.AffectedProvince];
            ProvinceState newState = new ProvinceState(war.Winner, oldState.Upgrades, oldState.Identifier, oldState.Tiles);
            oldNewDictionary[war.AffectedProvince] = newState;
        }
        return new GameState(oldNewDictionary.Values);
    }

    private class WarSorter
    {
        public IEnumerable<FinalWarsSetup> FinalWars { get; }

        public WarSorter(WarForcesTable forcesAllocator, IEnumerable<AttackMove> attackMoves)
        {
            Dictionary<Province, List<AttackMove>> byTargetTable = SortByTarget(attackMoves);
            IEnumerable<FinalWarsSetup> finalWars = GetFinalWars(byTargetTable, forcesAllocator).ToArray();
            FinalWars = finalWars.ToArray();
        }

        private IEnumerable<FinalWarsSetup> GetFinalWars(Dictionary<Province, List<AttackMove>> byTargetTable, WarForcesTable forcesTable)
        {
            foreach (KeyValuePair<Province, List<AttackMove>> item in byTargetTable)
            {
                WarForces defender = forcesTable.GetForcesFor(item.Key);
                IEnumerable<WarForces> invaders = GetAttackersForProvince(item.Key, item.Value, forcesTable).ToArray();
                FinalWarsSetup retItem = new FinalWarsSetup(defender, invaders);
                yield return retItem;
            }
        }

        private IEnumerable<WarForces> GetAttackersForProvince(Province defender, IEnumerable<AttackMove> invaders, WarForcesTable forcesTable)
        {
            IEnumerable<IEnumerable<AttackMove>> invadersByFaction = GetInvadersByFaction(invaders);
            foreach (IEnumerable<AttackMove> invadingFaction in invaders)
            {
                WarForces retItem = forcesTable.GetForcesFor(invadingFaction);
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

        public FinalWarsSetup(WarForces defender, IEnumerable<WarForces> invaders)
        {
            Wars = GetWars(defender, invaders);
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

        private static IEnumerable<War> GetWars(WarForces defender, IEnumerable<WarForces> invaders)
        {
            List<War> ret = new List<War>();
            foreach (WarForces invader in invaders)
            {
                War war = new War(defender, invader);
                ret.Add(war);
            }
            return ret;
        }
    }

    private class WarForcesTable
    {
        private ReadOnlyDictionary<AttackMove, WarForces> _attackerForces;

        private ReadOnlyDictionary<Province, WarForces> _defenderForces;

        public WarForcesTable(GameState state, IEnumerable<AttackMove> attackMoves)
        {
            _attackerForces = GetAttackerForces(state, attackMoves);
            _defenderForces = GetDefenderForces(state, attackMoves);
        }

        private WarForces CalculateDefendingForces(ProvinceState target, int attacksByDefender, GameState state)
        {
            // TODO: Calculate defending forces using these factors
            return new WarForces(target.Owner);
        }

        private WarForces CalculateAttackingForces(AttackMove move, int attacksByFaction, GameState state)
        {
            // TODO: Calculate war forces using these factors
            return new WarForces(move.Faction);
        }

        private ReadOnlyDictionary<Province, WarForces> GetDefenderForces(GameState state, IEnumerable<AttackMove> attacks)
        {
            Dictionary<Province, WarForces> ret = new Dictionary<Province, WarForces>();
            HashSet<Province> targets = new HashSet<Province>(attacks.Select(item => item.TargetProvince));
            foreach (Province target in targets)
            {
                ProvinceState provinceOwner = state.GetProvinceState(target);
                int attacksByDefender = attacks.Count(item => item.Faction == provinceOwner.Owner);
                WarForces forces = CalculateDefendingForces(provinceOwner, attacksByDefender, state);
                ret.Add(target, forces);
            }
            return new ReadOnlyDictionary<Province, WarForces>(ret);
        }

        private ReadOnlyDictionary<AttackMove, WarForces> GetAttackerForces(GameState state, IEnumerable<AttackMove> attackMoves)
        {
            Dictionary<AttackMove, WarForces> ret = new Dictionary<AttackMove, WarForces>();
            foreach (AttackMove move in attackMoves)
            {
                int attacksByFaction = attackMoves.Count(item => item.Faction == move.Faction);
                WarForces forces = CalculateAttackingForces(move, attacksByFaction, state);
                ret.Add(move, forces);
            }
            return new ReadOnlyDictionary<AttackMove, WarForces>(ret);
        }

        public WarForces GetForcesFor(AttackMove attackMove)
        {
            return _attackerForces[attackMove];
        }
        public WarForces GetForcesFor(IEnumerable<AttackMove> convergingAttacks)
        {
            IEnumerable<WarForces> forces = convergingAttacks.Select(item => _attackerForces[item]);
            return WarForces.CombineForces(forces);
        }

        public WarForces GetForcesFor(Province defender)
        {
            return _defenderForces[defender];
        }
    }
}
