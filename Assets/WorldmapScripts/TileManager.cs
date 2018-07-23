using UnityEngine;

public class TileManager
{
    public Vector2 AscendingTileOffset { get; }
    private readonly Worldmap _map;
    private readonly Plane _groundPlane;
    private readonly int _tileLayermask;

    public TileManager(Worldmap map)
    {
        _tileLayermask = 1 << LayerMask.NameToLayer("TileLayer");
        _map = map;
        _groundPlane = new Plane(Vector3.up, 0);
        AscendingTileOffset = new Vector2(1, -1.73f).normalized;
    }

    public Vector3 GetProvincePosition(int row, int ascendingColumn)
    {
        Vector2 ascendingOffset = AscendingTileOffset * ascendingColumn;
        Vector2 offset = ascendingOffset + new Vector2(row, 0);
        offset *= 2;
        return new Vector3(offset.x, 0, offset.y);
    }

    public Tile GetTileUnderMouse()
    {
        return GetTileAtScreenPoint(Input.mousePosition);
    }

    public Tile GetTileAtScreenPoint(Vector3 pos)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(pos);
        float enter;
        if (_groundPlane.Raycast(mouseRay, out enter))
        {
            Vector3 mousePoint = mouseRay.origin + (mouseRay.direction * enter);

            Tile approximateTile = GetApproximateTile(new Vector2(mousePoint.x, mousePoint.z));
            return GetClosestTile(mouseRay, approximateTile);
        }
        return null;
    }

    private Tile GetClosestTile(Ray ray, Tile approximateTile)
    {
        foreach (MeshCollider collider in approximateTile.ColliderDictionary.Keys)
        {
            collider.enabled = true;
        }
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _tileLayermask, QueryTriggerInteraction.UseGlobal);
        foreach (MeshCollider collider in approximateTile.ColliderDictionary.Keys)
        {
            collider.enabled = false;
        }
        if(hit)
        {
            return approximateTile.ColliderDictionary[hitInfo.collider];
        }
        return null;
    }

    private Tile GetApproximateTile(Vector2 pos)
    {
        Vector2 intersection = FindIntersection(pos, pos + AscendingTileOffset, Vector2.zero, Vector2.right);
        int row = (int)(intersection.x / 2 + .5f);

        Tile rowTile = _map.GetTile(row, 0);
        Vector2 basePoint = new Vector2(rowTile.transform.position.x, rowTile.transform.position.z);
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