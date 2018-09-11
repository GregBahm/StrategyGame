using System;
using System.Collections.Generic;
using System.Linq;

public class ArmyMovesResolver
{
    public GameState NewGameState { get; }

    public ArmyMovesResolver(GameState oldState, IEnumerable<ArmyMove> armyMoves)
    {
        // First, find any situations where two armies try to move into each others provinces.
        CollisionsSorter collisionsSorter = new CollisionsSorter(armyMoves);

        CollisionApplier collisionApplier = new CollisionApplier(oldState, collisionsSorter);

        // Resolve those first and get the new game state where only one army is attacking. 
        FinalCombatSetup finalFights = new FinalCombatSetup(collisionApplier);

        // Then resolve the remaining fights and moves
        GameState postPeacefulMoves = GetPostPeacefulMoves(oldState, finalFights.PeacefulTransitions);
        NewGameState = GetPostFinalFights(postPeacefulMoves, finalFights.FinalFights);
    }

    private GameState GetPostPeacefulMoves(GameState state, IEnumerable<ArmyMove> peacefulTransitions)
    {
        Dictionary<Guid, ArmyState> armyDictionary = state.Armies.ToDictionary(item => item.Identifier, item => item);
        foreach (ArmyMove move in peacefulTransitions)
        {
            ArmyState newArmy = new ArmyState(move.Army.Identifier, move.TargetProvince.Identifier, move.Army.Forces, move.Army.Routed);
            armyDictionary[newArmy.Identifier] = newArmy;
        }
        return new GameState(state.Provinces, armyDictionary.Values);
    }

    private static GameState GetPostFinalFights(GameState state, IEnumerable<CombatSetup> fights)
    {
        Dictionary<Guid, ProvinceState> provinceDictionary = state.Provinces.ToDictionary(item => item.Identifier, item => item);
        Dictionary<Guid, ArmyState> armyDictionary = state.Armies.ToDictionary(item => item.Identifier, item => item);
        foreach (CombatSetup fight in fights)
        {
            foreach (CombatOutcome outcome in fight.Outcome)
            {
                Guid armyGuid = outcome.ArmyAfterCombat.Identifier;
                armyDictionary[armyGuid] = outcome.ArmyAfterCombat;

                if(outcome.Victorious)
                {
                    ProvinceState oldProvince = outcome.ArmyBeforeCombat.TargetProvince;
                    Faction faction = outcome.ArmyBeforeCombat.Faction;
                    ProvinceState province = GetChangedOwnership(oldProvince, faction);
                    provinceDictionary[province.Identifier] = province;
                }
            }
        }
        return new GameState(provinceDictionary.Values, armyDictionary.Values);
    }

    private static ProvinceState GetChangedOwnership(ProvinceState oldProvince, Faction faction)
    {
        return new ProvinceState(faction, oldProvince.Upgrades, oldProvince.Identifier, oldProvince.Tiles);
    }

    private class CollisionsSorter
    {
        public IEnumerable<ArmyMove> NotCollisions { get; }
        public IEnumerable<CombatSetup> Collisions { get; }   

        public CollisionsSorter(IEnumerable<ArmyMove> armyMoves)
        {
            List<ArmyMove> notCollisions = new List<ArmyMove>();
            List<CombatSetup> collisions = new List<CombatSetup>();
            foreach (ArmyMove move in armyMoves)
            {
                ArmyMove collision = NotCollisions.FirstOrDefault(item => IsCollision(item, move));
                if(collision == null)
                {
                    notCollisions.Add(move);
                }
                else
                {
                    notCollisions.Remove(collision);
                    CombatSetup setup = new CombatSetup(new ArmyMove[] { collision, move }, new ArmyState[0], null);
                    collisions.Add(setup);
                }
            }
            NotCollisions = notCollisions;
            Collisions = collisions;
        }

        private bool IsCollision(ArmyMove armyA, ArmyMove armyB)
        {
            bool attack = armyA.Faction != armyB.Faction;
            if(attack)
            {
                bool abAtttack = armyA.TargetProvince.Identifier == armyB.Army.LocationId;
                bool baAttack = armyB.TargetProvince.Identifier == armyA.Army.LocationId;
                return abAtttack && baAttack;
            }
            return false;
        }
    }

    private class CollisionApplier // Happens between collision sorter and post collisions
    {
        public GameState NewGameState { get; }
        public IEnumerable<ArmyMove> MovesToGo { get; }

        public CollisionApplier(GameState oldState, CollisionsSorter collisionsSorter)
        {
            // Go through each collision. 
            // Change each army in the state to the  
            // If it's a winner, add the army move. 

            Dictionary<Guid, ArmyState> newArmiesDictionary = oldState.Armies.ToDictionary(item => item.Identifier, item => item);
            List<ArmyMove> newMoves = new List<ArmyMove>(collisionsSorter.NotCollisions);

            foreach (CombatSetup fight in collisionsSorter.Collisions)
            {
                foreach (CombatOutcome outcome in fight.Outcome)
                {
                    if(outcome.ArmyAfterCombat == null)
                    {
                        newArmiesDictionary.Remove(outcome.ArmyBeforeCombat.Army.Identifier);
                    }
                    else
                    {
                        newArmiesDictionary[outcome.ArmyAfterCombat.Identifier] = outcome.ArmyAfterCombat;
                    }
                }
                CombatOutcome winner = fight.Outcome.FirstOrDefault(item => item.Victorious);
                if (winner != null)
                {
                    ArmyMove newMove = new ArmyMove(winner.ArmyBeforeCombat.Faction, winner.ArmyAfterCombat, winner.ArmyBeforeCombat.TargetProvince);
                    newMoves.Add(newMove);
                }
            }
            NewGameState = new GameState(oldState.Provinces, newArmiesDictionary.Values);
        }
    }

    private class FinalCombatSetup
    {
        public IEnumerable<ArmyMove> PeacefulTransitions { get; }
        public IEnumerable<CombatSetup> FinalFights { get; }

        public FinalCombatSetup(CollisionApplier collisionApplier)
        {
            IEnumerable<List<ArmyMove>> convergencies = GetConvergencies(collisionApplier);

            List<ArmyMove> peacefulTransitions = new List<ArmyMove>();
            List<CombatSetup> fights = new List<CombatSetup>();
            IEnumerable<ArmyState> stationaryArmies = GetStationaryArmies(collisionApplier);
            foreach (IEnumerable<ArmyMove> convergence in convergencies)
            {
                bool peaceful = GetIsPeaceful(convergence);
                if(peaceful)
                {
                    peacefulTransitions.AddRange(convergence);
                }
                else
                {
                    CombatSetup combatSetup = GetCombatSetup(convergence, stationaryArmies);
                    fights.Add(combatSetup);
                }
            }

            PeacefulTransitions = peacefulTransitions;
            FinalFights = fights;
        }

        private IEnumerable<ArmyState> GetStationaryArmies(CollisionApplier collisionApplier)
        {
            IEnumerable<Guid> movingArmiesGuids = collisionApplier.MovesToGo.Select(item => item.Army.Identifier);
            HashSet<Guid> movingArmiesHash = new HashSet<Guid>(movingArmiesGuids);
            return collisionApplier.NewGameState.Armies.Where(item => !movingArmiesHash.Contains(item.Identifier));
        }

        private CombatSetup GetCombatSetup(IEnumerable<ArmyMove> convergence, IEnumerable<ArmyState> stationaryArmies)
        {
            ProvinceState defendingProvince = convergence.First().TargetProvince;
            IEnumerable<ArmyState> immobileForces = stationaryArmies.Where(item => item.LocationId == defendingProvince.Identifier);
            return new CombatSetup(convergence, immobileForces, defendingProvince);
        }
        
        private bool GetIsPeaceful(IEnumerable<ArmyMove> convergence)
        {
            Faction firstFaction = convergence.First().TargetProvince.Owner;
            return convergence.All(item => item.Faction == firstFaction);
        }

        private IEnumerable<List<ArmyMove>> GetConvergencies(CollisionApplier collisionApplier)
        {
            Dictionary<Guid, List<ArmyMove>> ret = new Dictionary<Guid, List<ArmyMove>>();

            foreach (ArmyMove item in collisionApplier.MovesToGo)
            {
                Guid provinceId = item.TargetProvince.Identifier;
                if(ret.ContainsKey(provinceId))
                {
                    ret[provinceId].Add(item);
                }
                else
                {
                    ret.Add(provinceId, new List<ArmyMove>() { item });
                }
            }
            return ret.Values;
        }
    }
}
