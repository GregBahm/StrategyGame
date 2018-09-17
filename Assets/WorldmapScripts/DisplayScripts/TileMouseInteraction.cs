using System.Collections.Generic;
using UnityEngine;

public class TileMouseInteraction
{
    private readonly Worldmap _map;
    private readonly Plane _groundPlane;
    private readonly int _tileLayermask;

    public TileMouseInteraction(Worldmap map)
    {
        _tileLayermask = 1 << LayerMask.NameToLayer("TileLayer");
        _map = map;
        _groundPlane = new Plane(Vector3.up, 0);
    }

    public TileDisplay GetTileUnderMouse()
    {
        return GetTileAtScreenPoint(Input.mousePosition);
    }

    public TileDisplay GetTileAtScreenPoint(Vector3 pos)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(pos);
        float enter;
        if (_groundPlane.Raycast(mouseRay, out enter))
        {
            Vector3 mousePoint = mouseRay.origin + (mouseRay.direction * enter);

            TileDisplay approximateTile = GetApproximateTile(new Vector2(mousePoint.x, mousePoint.z));
            return GetClosestTile(mouseRay, approximateTile);
        }
        return null;
    }

    private TileDisplay GetClosestTile(Ray ray, TileDisplay approximateTile)
    {
        foreach (TileDisplay tile in approximateTile.ColliderCluster)
        {
            tile.Collider.enabled = true;
        }
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _tileLayermask, QueryTriggerInteraction.UseGlobal);
        foreach (TileDisplay tile in approximateTile.ColliderCluster)
        {
            tile.Collider.enabled = false;
        }
        if (hit)
        {
            return approximateTile.ColliderCluster[hitInfo.collider];
        }
        return null;
    }

    private TileDisplay GetApproximateTile(Vector2 pos)
    {
        Vector2 intersection = FindIntersection(pos, pos + _map.AscendingTileOffset, Vector2.zero, Vector2.right);
        int row = (int)(intersection.x / 2 + .5f);

        TileDisplay rowTile = _map.GetTile(row, 0);
        Vector2 basePoint = new Vector2(rowTile.GameObject.transform.position.x, rowTile.GameObject.transform.position.z);
        int column = (int)((pos - basePoint).magnitude / 2 + .5f);
        return _map.GetTile(row, column);
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
}