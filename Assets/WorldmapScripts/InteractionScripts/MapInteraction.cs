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
    public Army SelectingArmy { get; private set; }
    public Army DraggingArmy { get; private set; }

    public Tile HoveredTile { get; private set; }
    public Tile SelectedTile { get; private set; }
    public Tile SelectingTile { get; private set; }
    public Tile DraggingTile { get; private set; }

    public bool AnythingSelected { get { return SelectedTile != null || SelectedArmy != null; } }
    public bool AnythingHovered { get { return HoveredTile != null || HoveredArmy != null; } }
    public bool AnythingDragging { get { return DraggingTile != null || DraggingArmy != null; } }

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

    public void Update()
    {
        SetHover();
        bool mouseDown = Input.GetMouseButton(0);
        if (mouseDown)
        {
            bool mouseJustDown = Input.GetMouseButtonDown(0);
            if (mouseJustDown)
            {
                HandleSelectedToDragging();
                HandleHoveredToSelecting();
            }
        }
        else
        {
            bool mouseJustUp = Input.GetMouseButtonUp(0);
            if (mouseJustUp)
            {
                HandleSelectingToSelected();
            }

            DraggingArmy = null;
            DraggingTile = null;
            SelectingArmy = null;
            SelectingTile = null;
        }
    }

    private void HandleSelectingToSelected()
    {
        if (HoveredTile == SelectingTile)
        {
            SelectedTile = SelectingTile;
        }
        if (HoveredArmy == SelectingArmy)
        {
            SelectedArmy = SelectingArmy;
        }
    }

    private void HandleSelectedToDragging()
    {
        if (HoveredTile == SelectedTile)
        {
            DraggingTile = SelectedTile;
        }
        if (HoveredArmy == SelectedArmy)
        {
            DraggingArmy = SelectedArmy;
        }
    }

    private void HandleHoveredToSelecting()
    {
        if (HoveredTile != null)
        {
            SelectingTile = HoveredTile;
        }
        if (HoveredArmy != null)
        {
            SelectingArmy = HoveredArmy;
        }
    }

    private void SetHover()
    {
        HoveredArmy = GetArmyHover();
        if (HoveredArmy != null)
        {
            HoveredTile = null;
        }
        else
        {
            HoveredTile = GetTileUnderMouse();
        }
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

    private Tile GetTileAtScreenPointOld(Vector3 pos)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(pos);
        float enter;
        if (_groundPlane.Raycast(mouseRay, out enter))
        {
            Vector3 mousePoint = mouseRay.origin + (mouseRay.direction * enter);

            TileUnityObject approximateTile = GetApproximateTile(new Vector2(mousePoint.x, mousePoint.z));
            return GetClosestTile(mouseRay, approximateTile);
        }
        return null;
    }

    private Tile GetClosestTile(Ray ray, TileUnityObject approximateTile)
    {
        foreach (TileUnityObject tile in approximateTile.ColliderCluster)
        {
            tile.Collider.enabled = true;
        }
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _tileLayermask, QueryTriggerInteraction.UseGlobal);
        foreach (TileUnityObject tile in approximateTile.ColliderCluster)
        {
            tile.Collider.enabled = false;
        }
        if (hit)
        {
            return approximateTile.ColliderCluster[hitInfo.collider].Tile;
        }
        return null;
    }

    private TileUnityObject GetApproximateTile(Vector2 pos)
    {
        Vector2 intersection = FindIntersection(pos, pos + MapDisplay.AscendingTileOffset, Vector2.zero, Vector2.right);

        int row = (int)(intersection.x / 2 + .5f);

        Tile rowTile = _map.GetTile(row, 0);
        TileUnityObject rowTileObject = _objectManager.GetUnityObject(rowTile);
        Vector2 basePoint = new Vector2(rowTileObject.transform.position.x, rowTileObject.transform.position.z);

        int column = (int)((pos - basePoint).magnitude / 2 + .5f);

        Tile retTile = _map.GetTile(row, column);
        return _objectManager.GetUnityObject(retTile);
    }

    private Vector2 FindIntersection(Vector2 lineAStart, Vector2 lineAEnd, Vector2 lineBStart, Vector2 lineBEnd)
    {
        Vector2 lineADiff = lineAEnd - lineAStart;
        Vector2 lineBDiff = lineBEnd - lineBStart;

        float denominator = (lineADiff.y * lineBDiff.x - lineADiff.x * lineBDiff.y);

        float t1 = ((lineAStart.x - lineBStart.x) * lineBDiff.y + (lineBStart.y - lineAStart.y) * lineBDiff.x) / denominator;
        float t2 = ((lineBStart.x - lineAStart.x) * lineADiff.y + (lineAStart.y - lineBStart.y) * lineADiff.x) / -denominator;

        return new Vector2(lineAStart.x + lineADiff.x * t1, lineAStart.y + lineADiff.y * t1);
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
