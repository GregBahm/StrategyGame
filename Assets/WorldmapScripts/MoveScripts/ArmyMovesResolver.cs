using System;
using System.Collections.Generic;
using System.Linq;

public class ArmyMovesResolver
{
    private readonly Dictionary<Army, Province> _destinationsDictionary;
    private readonly Dictionary<Army, ArmyStateHistory> _armyHistory;
    public GameState NewGameState { get; }

    public ArmyMovesResolver(GameState state, IEnumerable<ArmyMove> armyMoves)
    {
        _destinationsDictionary = GetDestinationsDictionary(state, armyMoves);
        // First, find any situations where two armies try to move into each others provinces.
        CollisionsSorter collisionsSorter = new CollisionsSorter(armyMoves, state);

        CollisionApplier collisionApplier = new CollisionApplier(state, collisionsSorter);

        // Resolve those first and get the new game state where only one army is attacking. 
        FinalCombatSetup finalFights = new FinalCombatSetup(collisionApplier);

        // Then resolve the remaining fights and moves
        GameState postPeacefulMoves = GetPostPeacefulMoves(state, finalFights.PeacefulTransitions);
        NewGameState = GetPostFinalFights(postPeacefulMoves, finalFights.FinalFights);
        _armyHistory = GetArmyHistory(state, collisionApplier.NewGameState, collisionsSorter, finalFights);
    }

    private Dictionary<Army, ArmyStateHistory> GetArmyHistory(GameState startingState, 
        GameState postCollisionState, 
        CollisionsSorter collisionsSorter,
        FinalCombatSetup finalFights)
    {
        HashSet<Army> nonCollisions = new HashSet<Army>(collisionsSorter.NotCollisions.Select(item => item.Army));
        Dictionary<Army, ArmyStateHistory> ret = new Dictionary<Army, ArmyStateHistory>();
        foreach (ArmyState armyState in startingState.Armies)
        {
            bool foughtInCollision = !nonCollisions.Contains(armyState.Identifier);
            ArmyState postCollision = postCollisionState.GetArmyState(armyState.Identifier);
            bool foughtInNonCollision = finalFights.GetDidFight(armyState.Identifier);
            ArmyState postNonCollision = NewGameState.GetArmyState(armyState.Identifier);
            ArmyStateHistory history = new ArmyStateHistory(foughtInCollision,
                foughtInNonCollision,
                postCollision,
                postNonCollision);
        }
        return ret;
    }

    private Dictionary<Army, Province> GetDestinationsDictionary(GameState oldState, IEnumerable<ArmyMove> armyMoves)
    {
        Dictionary<Army, Province> ret = oldState.Armies.ToDictionary(item => item.Identifier, item => item.LocationId);
        foreach (ArmyMove move in armyMoves)
        {
            ret[move.Army] = move.TargetProvince;
        }
        return ret;
    }

    public Province GetArmyDestination(Army army)
    {
        return _destinationsDictionary[army];
    }

    public ArmyStateHistory GetArmyHistory(Army army)
    {
        return _armyHistory[army];
    }
    
    private GameState GetPostPeacefulMoves(GameState state, IEnumerable<ArmyMove> peacefulTransitions)
    {
        Dictionary<Army, ArmyState> armyDictionary = state.Armies.ToDictionary(item => item.Identifier, item => item);
        foreach (ArmyMove move in peacefulTransitions)
        {
            ArmyState oldArmyState = state.GetArmyState(move.Army);
            ArmyState newArmyState = new ArmyState(move.Army, move.TargetProvince, oldArmyState.Forces, oldArmyState.Routed);
            armyDictionary[newArmyState.Identifier] = newArmyState;
        }
        return new GameState(state.Provinces, armyDictionary.Values);
    }

    private static GameState GetPostFinalFights(GameState state, IEnumerable<CombatSetup> fights)
    {
        Dictionary<Province, ProvinceState> provinceDictionary = state.Provinces.ToDictionary(item => item.Identifier, item => item);
        Dictionary<Army, ArmyState> armyDictionary = state.Armies.ToDictionary(item => item.Identifier, item => item);
        foreach (CombatSetup fight in fights)
        {
            foreach (CombatOutcome outcome in fight.Outcome)
            {
                Army armyGuid = outcome.ArmyAfterCombat.Identifier;
                armyDictionary[armyGuid] = outcome.ArmyAfterCombat;

                if(outcome.Victorious)
                {
                    ProvinceState oldProvince = state.GetProvinceState(outcome.ArmyBeforeCombat.TargetProvince);
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

        public CollisionsSorter(IEnumerable<ArmyMove> armyMoves, GameState state)
        {
            List<ArmyMove> notCollisions = new List<ArmyMove>();
            List<CombatSetup> collisions = new List<CombatSetup>();
            foreach (ArmyMove move in armyMoves)
            {
                ArmyMove collision = NotCollisions.FirstOrDefault(item => IsCollision(item, move, state));
                if(collision == null)
                {
                    notCollisions.Add(move);
                }
                else
                {
                    notCollisions.Remove(collision);
                    CombatSetup setup = new CombatSetup(state, new ArmyMove[] { collision, move }, new ArmyState[0], null);
                    collisions.Add(setup);
                }
            }
            NotCollisions = notCollisions;
            Collisions = collisions;
        }

        private bool IsCollision(ArmyMove armyA, ArmyMove armyB, GameState state)
        {
            bool attack = armyA.Faction != armyB.Faction;
            if(attack)
            {
                Province armyALocation = state.GetArmyState(armyA.Army).LocationId;
                Province armyBLocation = state.GetArmyState(armyB.Army).LocationId;

                bool abAtttack = armyA.TargetProvince == armyBLocation;
                bool baAttack = armyB.TargetProvince == armyALocation;
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

            Dictionary<Army, ArmyState> newArmiesDictionary = oldState.Armies.ToDictionary(item => item.Identifier, item => item);
            List<ArmyMove> newMoves = new List<ArmyMove>(collisionsSorter.NotCollisions);

            foreach (CombatSetup fight in collisionsSorter.Collisions)
            {
                foreach (CombatOutcome outcome in fight.Outcome)
                {
                    if(outcome.ArmyAfterCombat == null)
                    {
                        newArmiesDictionary.Remove(outcome.ArmyBeforeCombat.Army);
                    }
                    else
                    {
                        newArmiesDictionary[outcome.ArmyAfterCombat.Identifier] = outcome.ArmyAfterCombat;
                    }
                }
                CombatOutcome winner = fight.Outcome.FirstOrDefault(item => item.Victorious);
                if (winner != null)
                {
                    ArmyMove newMove = new ArmyMove(winner.ArmyBeforeCombat.Faction, winner.ArmyAfterCombat.Identifier, winner.ArmyBeforeCombat.TargetProvince);
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
        private readonly HashSet<Army> _armiesThatFought;

        public FinalCombatSetup(CollisionApplier collisionApplier)
        {
            IEnumerable<List<ArmyMove>> convergencies = GetConvergencies(collisionApplier);
            List<ArmyMove> peacefulTransitions = new List<ArmyMove>();
            List<CombatSetup> fights = new List<CombatSetup>();
            IEnumerable<ArmyState> stationaryArmies = GetStationaryArmies(collisionApplier);
            foreach (IEnumerable<ArmyMove> convergence in convergencies)
            {
                bool peaceful = GetIsPeaceful(collisionApplier.NewGameState, convergence);
                if(peaceful)
                {
                    peacefulTransitions.AddRange(convergence);
                }
                else
                {
                    CombatSetup combatSetup = GetCombatSetup(collisionApplier.NewGameState, convergence, stationaryArmies);
                    fights.Add(combatSetup);
                }
            }

            PeacefulTransitions = peacefulTransitions;
            FinalFights = fights;

            _armiesThatFought = GetArmiesThatFought(fights);
        }

        private HashSet<Army> GetArmiesThatFought(List<CombatSetup> fights)
        {
            HashSet<Army> ret = new HashSet<Army>();
            foreach (Army fighter in fights.SelectMany(item => item.Participants))
            {
                ret.Add(fighter);
            }
            return ret;
        }

        public bool GetDidFight(Army army)
        {
            return _armiesThatFought.Contains(army);
        }

        private IEnumerable<ArmyState> GetStationaryArmies(CollisionApplier collisionApplier)
        {
            IEnumerable<Army> movingArmiesGuids = collisionApplier.MovesToGo.Select(item => item.Army);
            HashSet<Army> movingArmiesHash = new HashSet<Army>(movingArmiesGuids);
            return collisionApplier.NewGameState.Armies.Where(item => !movingArmiesHash.Contains(item.Identifier));
        }

        private CombatSetup GetCombatSetup(GameState state, IEnumerable<ArmyMove> convergence, IEnumerable<ArmyState> stationaryArmies)
        {
            Province defendingProvince = convergence.First().TargetProvince;
            ProvinceState defendingProvinceState = state.GetProvinceState(defendingProvince);
            IEnumerable<ArmyState> immobileForces = stationaryArmies.Where(item => item.LocationId == defendingProvince);
            return new CombatSetup(state, convergence, immobileForces, defendingProvinceState);
        }
        
        private bool GetIsPeaceful(GameState state, IEnumerable<ArmyMove> convergence)
        {
            Province province = convergence.First().TargetProvince;
            Faction firstFaction = state.GetProvinceState(province).Owner;
            return convergence.All(item => item.Faction == firstFaction);
        }

        private IEnumerable<List<ArmyMove>> GetConvergencies(CollisionApplier collisionApplier)
        {
            Dictionary<Province, List<ArmyMove>> ret = new Dictionary<Province, List<ArmyMove>>();

            foreach (ArmyMove item in collisionApplier.MovesToGo)
            {
                Province provinceId = item.TargetProvince;
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

    public class ArmyStateHistory
    {
        public bool FoughtInCollision { get; }
        public bool FoughtInNonCollision { get; }
        public ArmyState PostCollision { get; }
        public ArmyState PostNonCollision { get; }

        public ArmyStateHistory
            (
            bool foughtInCollision,
            bool foughtInNonCollision,
            ArmyState postCollision,
            ArmyState postNonCollision
            )
        {
            FoughtInCollision = foughtInCollision;
            FoughtInNonCollision = FoughtInNonCollision;
            PostCollision = postCollision;
            PostNonCollision = postNonCollision;
        }
    }
}
