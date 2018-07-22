using System;
using System.Collections;
using UnityEngine;


public class Worldmap : MonoBehaviour
{
    public Tile HighlitTile
    {
        get
        {
            return _highlitTile;
        }
        set
        {
            if(_highlitTile != value)
            {
                if(_highlitTile != null)
                {
                    _highlitTile.Highlit = false;
                }
                if(value != null)
                {
                    value.Highlit = true;
                }
                _highlitTile = value;
            }
        }
    }
    public Tile[] Tiles;
    public GameObject TilePrefab;
    public Transform MapUvs;
    [Range(0, 1)]
    public float TileMargin;
    public Color BackgroundColor;
    public Material SkyMat;

    
    private Faction _factionA;
    private Faction _factionB;
    private Faction _unclaimed;
    private Province _startingProvince;
    private Tile _highlitTile;

    private TileManager _tileManager;
    public int Rows;
    public int Columns;
    public int TilesCount { get { return Rows * Columns; } }


    private void Start()
    {
        _tileManager = new TileManager(this);
        Tiles = MakeTiles();
        foreach (Tile tile in Tiles)
        {
            tile.EstablishNeighbors();
        }
        _factionA = new Faction(Color.red);
        _factionB = new Faction(Color.blue);
        _unclaimed = new Faction(Color.white);
        _startingProvince = new Province(_unclaimed);
        SetInitialProvince();
    }

    private void SetInitialProvince()
    {
        foreach (Tile tile in Tiles)
        {
            tile.Province = _startingProvince;
        }
        GetTile(0, 0).Province = new Province(_factionA);
        GetTile(Rows / 2, Columns / 2).Province = new Province(_factionB);
    }

    private void Update()
    {
        HighlitTile = _tileManager.GetTileUnderMouse();
        Shader.SetGlobalFloat("_TileMargin", TileMargin);
        Shader.SetGlobalMatrix("_MapUvs", MapUvs.worldToLocalMatrix);
        Shader.SetGlobalColor("_SideColor", BackgroundColor);
        SkyMat.SetColor("_Tint", BackgroundColor);
    }

    private Tile[] MakeTiles()
    {
        Tile[] ret = new Tile[TilesCount];
        for (int row = 0; row < Rows; row++)
        {
            for (int ascendingColumn = 0; ascendingColumn < Columns; ascendingColumn++)
            {
                int index = (row * Columns) + ascendingColumn;
                ret[index] = CreateTile(row, ascendingColumn);
            }
        }
        return ret;
    }

    public Tile GetTile(int row, int ascendingColumn)
    {
        int modRow = MathMod(row, Rows);
        int modColumn = MathMod(ascendingColumn, Columns);
        int index = (modRow * Columns) + modColumn;
        if (index < 0 || index >= Tiles.Length)
        {
            throw new Exception("Bad index (" + index + ")");
        }
        return Tiles[index];
    }

    private static int MathMod(int value, int modolus)
    {
        return (Math.Abs(value * modolus) + value) % modolus;
    }

    private Tile CreateTile(int row, int ascendingColumn)
    {
        int descendingColumn = row + ascendingColumn;
        string providenceName = string.Format("Providence {0} {1} {2}", row, ascendingColumn, descendingColumn);
        GameObject obj = Instantiate(TilePrefab);
        obj.name = providenceName;
        Tile ret = obj.AddComponent<Tile>();
        ret.Map = this;
        ret.Row = row;
        ret.AscendingColumn = ascendingColumn;
        obj.transform.position = _tileManager.GetProvincePosition(row, ascendingColumn);
        return ret;
    }
}


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