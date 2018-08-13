using System.Collections.Generic;

public class CombatSetup
{
    public Army AttackingForces;
    public Army DefendingDefending;
}

public class ArmyMovesResolver
{
    public GameState NewGameState { get; }

    public ArmyMovesResolver(GameState oldState, IEnumerable<ArmyMove> armyMoves)
    {
        // First, find any situations where two armies try to move into each others provinces.
        List<ArmyMove> notCollisions = new List<ArmyMove>();
        List<CombatSetup> collision = new List<CombatSetup>();
        foreach (ArmyMove move in armyMoves)
        {
        }

        // Resolve those first and get the new game state where only one army is attacking. 

        // Then move each army that that isn't attacking.

        // Then resolve the remaining attacks.
    }
}
