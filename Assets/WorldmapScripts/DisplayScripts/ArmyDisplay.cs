using System;
using UnityEngine;

public class ArmyDisplay
{
    private readonly GameDisplayManager _mothership;

    public GameObject ArtContent { get; }

    public Army Identifier { get; }

    public ArmyDisplay(GameDisplayManager mothership, Army identifier)
    {
        _mothership = mothership;
        Identifier = identifier;
        ArtContent = MakeArtContent();
    }

    private GameObject MakeArtContent()
    {
        // TODO: Figure out how you're making art content
        throw new NotImplementedException();
    }

    internal void SetArmyAsDead()
    {
        ArtContent.SetActive(false);
    }

    internal void DisplayArmy(GameTurnTransition gameTurnTransition, ArmyTurnTransition transition, DisplayTimings progression)
    {
        ArtContent.SetActive(true);
        ArtContent.transform.position = GetForcesPosition(gameTurnTransition, transition, progression);
        DisplayForces(transition, progression);
    }

    private Vector3 GetPostMergeDestinationPos(GameTurnTransition gameTurnTransition, Province originalDestination)
    {
        Province postMerge = gameTurnTransition.MergeTable.GetPostMerged(originalDestination);
        ProvinceState postMergeState = gameTurnTransition.FinalState.GetProvinceState(postMerge);
        return GetProvincePosition(postMergeState);
    }
    private Vector3 GetPreMergeDestinationPos(GameTurnTransition gameTurnTransition, Province originalDestination)
    {
        ProvinceState preMergeState = gameTurnTransition.InitialState.GetProvinceState(originalDestination);
        return GetProvincePosition(preMergeState);
    }
    private Vector3 GetStartingPos(GameTurnTransition gameTurnTransition, Province start)
    {
        ProvinceState startState = gameTurnTransition.InitialState.GetProvinceState(start);
        return GetProvincePosition(startState);
    }

    private Vector3 GetForcesPosition(GameTurnTransition gameTurnTransition, ArmyTurnTransition transition, DisplayTimings progression)
    {
        Province start = transition.StartingState.LocationId;
        Province originlDestination = transition.ArmyDestination;

        if (progression.ProvinceMergers > 0)
        {
            // You will need to know the province where the army ended up
            // But their destination could have merged away
            Vector3 postMergePos = GetPostMergeDestinationPos(gameTurnTransition, originlDestination);

            if (progression.ProvinceMergers >= 1)
            {
                return postMergePos;
            }
            else
            {
                // You will need to place it from the unmerged to the merged province

                Vector3 preMergePos = GetPreMergeDestinationPos(gameTurnTransition, originlDestination);
                return Vector3.Lerp(preMergePos, postMergePos, progression.ProvinceMergers);
            }
        }
        else
        {
            Vector3 startingPos = GetStartingPos(gameTurnTransition, start);
            Vector3 destinationPos = GetPreMergeDestinationPos(gameTurnTransition, originlDestination);
            Vector3 collisionPos = (startingPos + destinationPos) / 2;

            Vector3 finalPos = Vector3.Lerp(startingPos, collisionPos, progression.ArmiesMoveToCollision);
            finalPos = Vector3.Lerp(finalPos, destinationPos, progression.ArmiesToDestination);
            return finalPos;
        }
    }

    private void DisplayForces(ArmyTurnTransition transition, DisplayTimings progression)
    {
        throw new NotImplementedException();
    }

    private Vector3 GetProvincePosition(ProvinceState provinceState)
    {
        return _mothership.GetAverageTilePosition(provinceState.Tiles);
    }
}
