using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class MapInteraction
{
    private readonly Plane _groundPlane;
    private readonly UnityObjectManager _objectManager;
    private readonly ReadOnlyDictionary<Collider, Tile> _collisionDictionary;
    private readonly Map _map;
    private readonly int _armyLayerMask;
    private readonly int _tileLayermask;

    public Army HoveredArmy { get; private set; }
    public Army SelectedArmy { get; private set; }
    public Army DraggingArmy { get; private set; }

    public Province HoveredProvince { get; private set; }
    public Province SelectedProvince { get; private set; }
    public Province DraggingProvince { get; private set; }
    public Province DraggedOnProvince { get; private set; }

    public bool AnythingSelected { get { return SelectedProvince != null || SelectedArmy != null; } }
    public bool AnythingHovered { get { return HoveredProvince != null || HoveredArmy != null; } }
    public bool AnythingDragging { get { return DraggingProvince != null || DraggingArmy != null; } }

    public MapInteraction(GameSetup gameSetup, Map map, UnityObjectManager objectManager)
    {
        _objectManager = objectManager;
        _collisionDictionary = CreateCollisionDictionary(objectManager);
        _map = map;
        _armyLayerMask = 1 << LayerMask.NameToLayer("ArmyLayer");
        _tileLayermask = 1 << LayerMask.NameToLayer("TileLayer");
        _groundPlane = new Plane(Vector3.up, 0);
    }

    private ReadOnlyDictionary<Collider, Tile> CreateCollisionDictionary(UnityObjectManager objectManager)
    {
        Dictionary<Collider, Tile> ret = objectManager.Tiles.ToDictionary(item => item.Collider, item => item.Tile);
        return new ReadOnlyDictionary<Collider, Tile>(ret);
    }

    public void Update(GameState currentGamestate, ProvinceNeighborsTable neighbors)
    {
        SetHover(currentGamestate);
        bool mouseDown = Input.GetMouseButton(0);
        if (mouseDown)
        {
            bool mouseJustDown = Input.GetMouseButtonDown(0);
            if (mouseJustDown)
            {
                HandleHoverToSelected();
                HandleSelectedToDragging();
            }
            HandleDraggedUpon();
        }
        else
        {
            DraggedOnProvince = null;
            DraggingArmy = null;
            DraggingProvince = null;
        }
    }

    private void HandleDraggedUpon()
    {
        if (HoveredProvince != null && DraggingProvince != HoveredProvince && AnythingDragging)
        {
            DraggedOnProvince = HoveredProvince;
        }
        else
        {
            DraggedOnProvince = null;
        }
    }

    private void HandleHoverToSelected()
    {
        if (HoveredProvince != null)
        {
            SelectedProvince = HoveredProvince;
            SelectedArmy = null;
        }
        if (HoveredArmy != null)
        {
            SelectedArmy = HoveredArmy;
            SelectedProvince = null;
        }
    }

    private void HandleSelectedToDragging()
    {
        if (HoveredProvince == SelectedProvince)
        {
            DraggingProvince = SelectedProvince;
        }
        if (HoveredArmy == SelectedArmy)
        {
            DraggingArmy = SelectedArmy;
        }
    }

    private void SetHover(GameState currentGamestate)
    {
        HoveredArmy = GetArmyHover();
        if (HoveredArmy != null)
        {
            HoveredProvince = null;
        }
        else
        {
            HoveredProvince = GetProvinceUnderMouse(currentGamestate);
        }
    }

    private Province GetProvinceUnderMouse(GameState currentGamestate)
    {
        Tile tile = GetTileUnderMouse();
        if(tile != null)
        {
            return currentGamestate.GetTilesProvince(tile).Identifier;
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

    private Army GetArmyHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _armyLayerMask, QueryTriggerInteraction.UseGlobal);
        if (hit)
        {
            return hitInfo.transform.parent.gameObject.GetComponent<ArmyUnityObject>().Army;
        }
        return null;
    }

}
