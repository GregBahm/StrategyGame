using System;

public class ArmyTurnTransition
{
    /// <summary>
    /// The initial state of the army. Should never be null.
    /// </summary>
    public Army StartingState { get; }
    public Guid ArmyDestination { get; }
    public Army AfterCollisionFight { get; }
    public Army AfterNonCollisionFight { get; }
    public Army AfterReceiveUnits { get; }
    public bool IsNewThisTurn { get; }
    public bool FoughtInCollision { get; }
    public bool FoughtNonCollision { get; }

    public ArmyTurnTransition(
        Army startingState,
        Guid armyDestination,
        Army afterCollisionFight,
        Army afterNonCollisionFight,
        Army afterReceiveUnits,
        bool isNewThisTurn,
        bool foughtInCollision,
        bool foughtNonCOllision)
    {
        if(startingState == null)
        {
            throw new ArgumentNullException("startingState", "startingState of ArmyTurnTransition should never be null");
        }
        StartingState = startingState;
        ArmyDestination = armyDestination;
        AfterCollisionFight = afterCollisionFight;
        AfterNonCollisionFight = afterNonCollisionFight;
        AfterReceiveUnits = afterReceiveUnits;
        IsNewThisTurn = isNewThisTurn;
        FoughtInCollision = foughtInCollision;
        FoughtNonCollision = foughtNonCOllision;
    }
}
