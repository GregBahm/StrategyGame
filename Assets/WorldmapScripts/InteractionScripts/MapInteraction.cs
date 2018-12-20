using System;
using UnityEngine;

public class MapInteraction
{
    private readonly Plane _groundPlane;
    private readonly UnityObjectManager _objectManager;
    private readonly Map _map;
    private readonly int _armyLayerMask;
    private readonly int _tileLayermask;

    public Army HoveredArmy { get; private set; }
    public Army SelectedArmy { get; private set; }
    public Tile HoveredTile { get; private set; }
    public Tile SelectedTile { get; private set; }

    public MapInteraction(GameSetup gameSetup, Map map, UnityObjectManager objectManager)
    {
        _objectManager = objectManager;
        _map = map;
        _armyLayerMask =  1 << LayerMask.NameToLayer("ArmyLayer");
        _tileLayermask = 1 << LayerMask.NameToLayer("TileLayer");
        _groundPlane = new Plane(Vector3.up, 0);
    }

    public void Update()
    {
        SetHover();
        SetSelected();
    }

    private Tile GetTileUnderMouse()
    {
        return GetTileAtScreenPoint(Input.mousePosition);
    }

    private Tile GetTileAtScreenPoint(Vector3 pos)
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
        row = Mathf.Clamp(row, 0, _map.Rows - 1);
        Tile rowTile = _map.GetTile(row, 0);
        TileUnityObject rowTileObject = _objectManager.GetUnityObject(rowTile);
        Vector2 basePoint = new Vector2(rowTileObject.transform.position.x, rowTileObject.transform.position.z);
        int column = (int)((pos - basePoint).magnitude / 2 + .5f);
        column = Mathf.Clamp(column, 0, _map.Columns - 1);
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

    private void SetSelected()
    {
        bool mouseDown = Input.GetMouseButton(0);
        if (mouseDown)
        {
            if(HoveredArmy != null)
            {
                SelectedArmy = HoveredArmy;
            }
            if(HoveredTile != null)
            {
                SelectedTile = HoveredTile;
            }
        }
    }

    private Army GetArmyHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _armyLayerMask, QueryTriggerInteraction.UseGlobal);
        if(hit)
        {
            return hitInfo.transform.parent.gameObject.GetComponent<ArmyUnityObject>().Army;
        }
        return null;
    }
    
}
