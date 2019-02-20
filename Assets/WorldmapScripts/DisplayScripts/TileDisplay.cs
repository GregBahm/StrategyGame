using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class TileDisplay
{
    private readonly MapDisplay _mapDisplay;

    public Tile Tile { get; }

    public float Hover { get; private set; }
    public float Selected { get; private set; }
    public float Dragging { get; private set; }
    public float Dragged { get; private set; }
    public float Targetable { get; private set; }
    public Color FactionDisplayColor { get; private set; }

    public TileDisplay(Tile tile, MapDisplay map)
    {
        Tile = tile;
        _mapDisplay = map;
    }

    public TileDisplay GetOffset(int rowOffset, int ascendingColumnOffset)
    {
        return _mapDisplay.GetTile(Tile.Row + rowOffset, Tile.AscendingColumn + ascendingColumnOffset);
    }

    public void DisplayTile(GameTurnTransition gameTransition, DisplayTimings timings)
    {
        DisplayProvinceOwnershipChanges(gameTransition.BeforeEverything, gameTransition.AfterWars, timings.ProvinceOwnershipChanges);
        DisplayProvinceMergers(gameTransition.BeforeEverything, gameTransition.AfterEverything, timings.ProvinceMergers);
    }

    private void DisplayProvinceOwnershipChanges(GameState preOwnerGame, GameState postOwnerGame, float progression)
    {
        ProvinceState preOwnerProvince = preOwnerGame.GetTilesProvince(Tile);
        ProvinceState postOwnerProvince = postOwnerGame.GetTilesProvince(Tile);
        Color preColor = preOwnerProvince.Owner.Color;
        Color postColor = preOwnerProvince.Owner.Color;
        FactionDisplayColor = Color.Lerp(preColor, postColor, progression);
    }

    private void DisplayProvinceMergers(GameState preMergeGame, GameState postMergeGame, float mergerProgress)
    {
        Province preMergeOwner = preMergeGame.GetTilesProvince(Tile).Identifier;
        Province postMergeOwner = postMergeGame.GetTilesProvince(Tile).Identifier;
        //TODO: The rest of this
    }

    public void UpdateHighlighting(GameState gameState, 
        MapInteraction mapInteraction, 
        float transitionSpeed, 
        float timeDelta, 
        ProvinceNeighborsTable neighbors)
    {
        Province myProvince = gameState.GetTilesProvince(Tile).Identifier;
        bool isNeighborSelected = mapInteraction.OwnedProvinceSelected && 
            GetIsNeighborSelected(gameState, neighbors, mapInteraction, myProvince);

        float speed = transitionSpeed * timeDelta;

        bool isHovered = mapInteraction.HoveredProvince == myProvince;
        bool isSelected = mapInteraction.SelectedProvince == myProvince;
        bool isDragging = isSelected && mapInteraction.Dragging;
        bool isDragged = mapInteraction.DraggedOnProvince == myProvince;
        Hover = Mathf.Lerp(Hover, isHovered ? 1 : 0, speed);
        Selected = Mathf.Lerp(Selected, isSelected ? 1 : 0, speed);
        Dragging = Mathf.Lerp(Dragging, isDragging ? 1 : 0, speed);
        Dragged = Mathf.Lerp(Dragged, isDragged ? 1 : 0, speed);
        Targetable = Mathf.Lerp(Targetable, isNeighborSelected ? 1 : 0, speed);
    }

    private bool GetIsNeighborSelected(GameState gameState, 
        ProvinceNeighborsTable neighborsTable, 
        MapInteraction mapInteraction, 
        Province myProvince)
    {
        if(mapInteraction.SelectedProvince != null)
        {
            HashSet<Province> neighbors = neighborsTable.GetNeighborsFor(mapInteraction.SelectedProvince);
            return neighbors.Contains(myProvince);
        }
        return false;
    }
}
