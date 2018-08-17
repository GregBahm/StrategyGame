using System;
using System.Collections.Generic;
using System.Linq;

public class CombatOutcome
{
    public ArmyMove ArmyBeforeCombat { get; }
    public bool Victorious { get; }
    public Army ArmyAfterCombat { get; }

    public CombatOutcome(ArmyMove armyBeforeCombat,
        bool victorious,
        Army armyAfterCombat)
    {
        ArmyBeforeCombat = armyBeforeCombat;
        Victorious = victorious;
        ArmyAfterCombat = armyAfterCombat;
    }
}

public class CombatSetup
{
    public IEnumerable<CombatOutcome> Outcome { get; }

    public CombatSetup(IEnumerable<ArmyMove> attackers, ArmyMove defendingArmy)
    {
        // TODO: Sort out how combat rounds are built and resolved
    }
}

public class CombatRound
{
    public ArmyForces AttackingForces { get; }
    public ArmyForces DefendingForces { get; }

    public ArmyForces DefendingForcesAfterAttack { get; }

    public CombatRound(ArmyForces attackingForces, ArmyForces defendingForces)
    {
        AttackingForces = attackingForces;
        DefendingForces = defendingForces;
        DefendingForcesAfterAttack = GetForcesAfterAttack();
    }

    private ArmyForces GetForcesAfterAttack()
    {
        throw new NotImplementedException();
    }
}


public class ArmyMovesResolver
{
    public GameState NewGameState { get; }

    public ArmyMovesResolver(GameState oldState, IEnumerable<ArmyMove> armyMoves)
    {
        // First, find any situations where two armies try to move into each others provinces.
        CollisionsSort collisionsSort = new CollisionsSort(armyMoves);

        // Resolve those first and get the new game state where only one army is attacking. 
        PostCollisions postCollisions = new PostCollisions(collisionsSort);

        // Then resolve the remaining fights and moves
        GameState postPeacefulMoves = GetPostPeacefulMoves(postCollisions.State, postCollisions.RemainingTransitions);
        NewGameState = GetPostFinalFights(postPeacefulMoves, postCollisions.RemainingFights);
    }

    private GameState GetPostFinalFights(GameState postPeacefulMoves, IEnumerable<CombatSetup> remainingFights)
    {
        throw new NotImplementedException();
    }

    private GameState GetPostPeacefulMoves(GameState state, IEnumerable<ArmyMove> peacefulTransitions)
    {
        throw new NotImplementedException();
    }

    private class CollisionsSort
    {
        public List<ArmyMove> NotCollisions { get; } = new List<ArmyMove>();
        public List<CombatSetup> Collisions { get; } = new List<CombatSetup>();

        public CollisionsSort(IEnumerable<ArmyMove> armyMoves)
        {
            foreach (ArmyMove move in armyMoves)
            {
                ArmyMove collision = NotCollisions.FirstOrDefault(item => IsCollision(item, move));
                if(collision == null)
                {
                    NotCollisions.Add(move);
                }
                else
                {
                    NotCollisions.Remove(collision);
                    CombatSetup setup = new CombatSetup(new ArmyMove[] { collision, move }, null);
                    Collisions.Add(setup);
                }
            }
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

    private class PostCollisions
    {
        public GameState State { get; }
        public IEnumerable<ArmyMove> RemainingTransitions { get; }
        public IEnumerable<CombatSetup> RemainingFights { get; }

        public PostCollisions(CollisionsSort collisionSort)
        {
            foreach (CombatSetup fight in collisionSort.Collisions)
            {
            }
        }
    }
}
