using System;

public class ArmyTurnTransition
{
    /// <summary>
    /// The initial state of the army. Should never be null.
    /// </summary>
    public ArmyState StartingState { get; }
    public ArmyState AfterReceiveUnits { get; }
    public Province ArmyDestination { get; }
    public ArmyState AfterCollisionFight { get; }
    public ArmyState AfterNonCollisionFight { get; }
    public bool IsNewThisTurn { get; }
    public bool FoughtInCollision { get; }
    public bool FoughtNonCollision { get; }

    public ArmyTurnTransition(
        ArmyState startingState,
        ArmyState afterReceiveUnits,
        Province armyDestination,
        ArmyState afterCollisionFight,
        ArmyState afterNonCollisionFight,
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
