using System;
using UnityEngine;

public class ArmyDisplay
{
    private readonly GameDisplayManager _mothership;
    private readonly Transform _transform;

    public readonly GameObject _artContent;
    private readonly Material _mat;

    public Army Identifier { get; }

    private float _hover;
    private float _selecting;
    private float _selected;
    private float _dragging;

    public ArmyDisplay(GameDisplayManager mothership, Army identifier, Transform transform, GameObject artContent)
    {
        _mothership = mothership;
        _transform = transform;
        Identifier = identifier;
        _artContent = artContent;
        _mat = artContent.GetComponent<MeshRenderer>().material;
    }

    internal void SetArmyAsDead()
    {
        _artContent.SetActive(false);
    }

    internal void DisplayArmy(GameTurnTransition gameTurnTransition, ArmyTurnTransition transition, DisplayTimings progression)
    {
        _artContent.SetActive(true);
        _transform.position = GetForcesPosition(gameTurnTransition, transition, progression);
        DisplayForces(transition, progression);
    }

    private Vector3 GetPostMergeDestinationPos(GameTurnTransition gameTurnTransition, Province originalDestination)
    {
        Province postMerge = gameTurnTransition.MergeTable.GetPostMerged(originalDestination);
        ProvinceState postMergeState = gameTurnTransition.PostMergersState.GetProvinceState(postMerge);
        return GetProvincePosition(postMergeState);
    }
    private Vector3 GetPreMergeDestinationPos(GameTurnTransition gameTurnTransition, Province originalDestination)
    {
        ProvinceState preMergeState = gameTurnTransition.InitialState.GetProvinceState(originalDestination);
        return GetProvincePosition(preMergeState);
    }

    public void UpdateUi(MapInteraction mapInteraction, UiAethetics aethetics, float timeDelta)
    {
        UpdateHighlighting(mapInteraction, aethetics.TransitionSpeed, timeDelta);
    }

    private void UpdateHighlighting(MapInteraction mapInteraction, float transitionSpeed, float timeDelta)
    {
        float speed = transitionSpeed * timeDelta;

        bool isHovered = mapInteraction.HoveredArmy == Identifier;
        bool isSelected = mapInteraction.SelectedArmy == Identifier;
        bool isDragging = mapInteraction.DraggingArmy == Identifier;
        bool isSelecting = mapInteraction.SelectingArmy == Identifier && isHovered;
        _hover = Mathf.Lerp(_hover, isHovered ? 1 : 0, speed);
        _selected = Mathf.Lerp(_selected, isSelected ? 1 : 0, speed);
        _selecting = Mathf.Lerp(_selecting, isSelecting ? 1 : 0, speed);
        _dragging = Mathf.Lerp(_dragging, isDragging ? 1 : 0, speed);
        _mat.SetFloat("_Hover", _hover);
        _mat.SetFloat("_Selected", _selected);
        _mat.SetFloat("_Selecting", _selecting);
        _mat.SetFloat("_Dragging", _dragging);
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
            Vector3 preMergePos = GetPreMergeDestinationPos(gameTurnTransition, originlDestination);
            return Vector3.Lerp(preMergePos, postMergePos, progression.ProvinceMergers);
            
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
        // TODO: Display forces
    }

    private Vector3 GetProvincePosition(ProvinceState provinceState)
    {
        return _mothership.GetAverageTilePosition(provinceState.Tiles);
    }
}
