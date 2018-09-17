using System;
using UnityEngine;

public class ArmyDisplay
{
    public GameDisplayManager Manager { get; }

    public GameObject ArtContent { get; }

    public Army Identifier { get; }

    public ArmyDisplay(GameDisplayManager manager, Army identifier)
    {
        Manager = manager;
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

    internal void DisplayArmy(ArmyTurnTransition transition, DisplayTimings progression)
    {
        ArtContent.SetActive(true);
        ArtContent.transform.position = GetForcesPosition(transition, progression);
        DisplayForces(transition, progression);
    }

    private Vector3 GetForcesPosition(ArmyTurnTransition transition, DisplayTimings progression)
    {
        Province startingProvince = transition.StartingState.LocationId;
        Province destinationProvince = transition.ArmyDestination;
        Vector3 startingPos = GetProvincePosition(startingProvince);
        Vector3 destinationPos = GetProvincePosition(destinationProvince);
        Vector3 collisionPos = (startingPos + destinationPos) / 2;

        Vector3 finalPos = Vector3.Lerp(startingPos, collisionPos, progression.ArmiesMoveToCollision);
        finalPos = Vector3.Lerp(finalPos, destinationPos, progression.ArmiesToDestination);
        return finalPos;
    }

    private void DisplayForces(ArmyTurnTransition transition, DisplayTimings progression)
    {
        throw new NotImplementedException();
    }

    private Vector2 GetProvincePosition(Province provinceId)
    {
        throw new NotImplementedException();
    }
}
