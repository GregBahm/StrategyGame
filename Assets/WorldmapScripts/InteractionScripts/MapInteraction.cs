using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapInteraction
{
    public InteractionManager InteractionMain { get; }
    private readonly ReadOnlyDictionary<Collider, Tile> _collisionDictionary;
    private readonly int _tileLayermask;

    private ProvinceState _hoveredProvince;
    public Province HoveredProvince { get { return _hoveredProvince?.Identifier; } }
    private ProvinceState _selectedProvince;
    public Province SelectedProvince { get { return _selectedProvince?.Identifier; } }
    public bool Dragging { get; private set; }
    private ProvinceState _draggedOnProvince;
    public Province DraggedOnProvince { get { return _draggedOnProvince?.Identifier; } }
    public bool OwnedProvinceSelected
    {
        get
        {
            if(SelectedProvince != null)
            {
                return _selectedProvince.Owner == InteractionMain.Factions.ActiveFaction;
            }
            return false;
        }
    }

    public MapInteraction(InteractionManager main, GameSetup gameSetup, UnityObjectManager objectManager)
    {
        InteractionMain = main;
        _collisionDictionary = CreateCollisionDictionary(objectManager);
        _tileLayermask = 1 << LayerMask.NameToLayer("UI");
    }

    private ReadOnlyDictionary<Collider, Tile> CreateCollisionDictionary(UnityObjectManager objectManager)
    {
        Dictionary<Collider, Tile> ret = objectManager.Tiles.ToDictionary(item => item.Collider, item => item.Tile);
        return new ReadOnlyDictionary<Collider, Tile>(ret);
    }

    public void Update(GameState currentGamestate, ProvinceNeighborsTable neighbors)
    {
        _hoveredProvince = GetProvinceUnderMouse(currentGamestate);
        bool mouseDown = Input.GetMouseButton(0);
        if (mouseDown)
        {
            bool mouseJustDown = Input.GetMouseButtonDown(0);
            if (mouseJustDown)
            {
                HandleHoverToSelected(currentGamestate);
                HandleSelectedToDragging();
            }
            HandleDraggedUpon();
        }
        else
        {
            HandleDragDrop(neighbors);
            _draggedOnProvince = null;
            Dragging = false;
        }
    }

    private void HandleDragDrop(ProvinceNeighborsTable neighbors)
    {
        bool validDrag = GetWasValidDragDrop(neighbors);
        if(validDrag)
        {
            InteractionMain.Factions.ActiveInteraction.RequestAttackOrMerge(_selectedProvince, _draggedOnProvince);
        }
    }

    private bool GetWasValidDragDrop(ProvinceNeighborsTable neighbors)
    {
        bool mouseJustUp = Input.GetMouseButtonUp(0);
        bool wasDragDrop = OwnedProvinceSelected
            && Dragging
            && _draggedOnProvince != null
            && _selectedProvince != _draggedOnProvince
            && mouseJustUp;

        if (wasDragDrop)
        {
            return neighbors.GetNeighborsFor(_selectedProvince.Identifier).Contains(_draggedOnProvince.Identifier);
        }
        return false;
    }

    private void HandleDraggedUpon()
    {
        if (HoveredProvince != null 
            && SelectedProvince != HoveredProvince 
            && Dragging)
        {
            _draggedOnProvince = _hoveredProvince;
        }
        else
        {
            _draggedOnProvince = null;
        }
    }

    private void HandleHoverToSelected(GameState currentGamestate)
    {
        if (HoveredProvince != null)
        {
            _selectedProvince = _hoveredProvince;
        }
    }

    private void HandleSelectedToDragging()
    {
        if (HoveredProvince == SelectedProvince)
        {
            Dragging = true;
        }
    }

    private ProvinceState GetProvinceUnderMouse(GameState currentGamestate)
    {
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            Tile tile = GetTileUnderMouse();
            if (tile != null)
            {
                return currentGamestate.GetTilesProvince(tile);
            }
        }
        return null;
    }

    private Tile GetTileUnderMouse()
    {
        return GetTileAtScreenPoint(Input.mousePosition);
    }

    private Tile GetTileAtScreenPoint(Vector3 pos)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(pos);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity, _tileLayermask, QueryTriggerInteraction.UseGlobal);
        if(hit)
        {
            return _collisionDictionary[hitInfo.collider];
        }
        return null;
    }
}
