using System;
using System.Collections;
using UnityEngine;

public class Worldmap : MonoBehaviour
{
    public Transform DebugSphere;
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


    private int _rows;
    private int _columns;
    private Faction _factionA;
    private Faction _factionB;
    private Faction _unclaimed;
    private Province _startingProvince;
    private Tile _highlitTile;
    private Plane _groundPlane;
    private Vector2 _ascendingOffset;

    private void Start()
    {
        _ascendingOffset = new Vector2(1, -1.73f).normalized;
        _groundPlane = new Plane(Vector3.up, 0);
        _rows = 20;
        _columns = 20;
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
        GetTile(_rows / 2, _columns / 2).Province = new Province(_factionB);
    }

    private void Update()
    {
        HighlitTile = GetHighlitTile();
        Shader.SetGlobalFloat("_TileMargin", TileMargin);
        Shader.SetGlobalMatrix("_MapUvs", MapUvs.worldToLocalMatrix);
        Shader.SetGlobalColor("_SideColor", BackgroundColor);
        SkyMat.SetColor("_Tint", BackgroundColor);
    }

    private Tile GetHighlitTile()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;
        if(_groundPlane.Raycast(mouseRay, out enter))
        {
            Vector3 intersectionPoint = mouseRay.origin + (mouseRay.direction * enter);
            return GetClosestTile(new Vector2(intersectionPoint.x, intersectionPoint.z));
        }
        return null;
    }

    private Tile GetClosestTile(Vector2 pos)
    {
        Vector2 intersection = FindIntersection(pos, pos + _ascendingOffset, Vector2.zero, Vector2.right);
        DebugSphere.transform.position = new Vector3(intersection.x, 0, intersection.y);
        int row = (int)(intersection.x / 2 + .5f);
        int column = (int)((pos - intersection).magnitude / 2 + .5f);
        return GetTile(row, column);
    }

    private Vector3 GetProvincePosition(int row, int ascendingColumn)
    {
        Vector2 ascendingOffset = _ascendingOffset * ascendingColumn;
        Vector2 offset = ascendingOffset + new Vector2(row, 0);
        offset *= 2;
        return new Vector3(offset.x, 0, offset.y);
    }

    private Tile[] MakeTiles()
    {
        Tile[] ret = new Tile[_rows * _columns];
        for (int row = 0; row < _rows; row++)
        {
            for (int ascendingColumn = 0; ascendingColumn < _columns; ascendingColumn++)
            {
                int index = (row * _columns) + ascendingColumn;
                ret[index] = CreateTile(row, ascendingColumn);
            }
        }
        return ret;
    }

    public Tile GetTile(int row, int ascendingColumn)
    {
        int modRow = MathMod(row, _rows);
        int modColumn = MathMod(ascendingColumn, _columns);
        int index = (modRow * _columns) + modColumn;
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
        obj.transform.position = GetProvincePosition(row, ascendingColumn);
        return ret;
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
