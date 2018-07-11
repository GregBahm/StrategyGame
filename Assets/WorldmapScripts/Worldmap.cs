using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worldmap : MonoBehaviour
{
    private int _rows;
    private int _columns;
    public Tile[] Tiles;
    public GameObject TilePrefab;
    public Transform MapUvs;
    [Range(0, 1)]
    public float TileMargin;
    public Color BackgroundColor;
    public Material SkyMat;

    private Province _startingProvince;
    private Province _testProvince;

    private void Start()
    {
        _rows = 20;
        _columns = 20;
        Tiles = MakeTiles();
        foreach (Tile tile in Tiles)
        {
            tile.EstablishNeighbors();
        }
        _startingProvince = new Province();
        _testProvince = new Province();
        SetInitialProvince();
    }

    private void SetInitialProvince()
    {
        foreach (Tile tile in Tiles)
        {
            tile.Province = _startingProvince;
        }
    }

    private void Update()
    {
        Shader.SetGlobalFloat("_TileMargin", TileMargin);
        Shader.SetGlobalMatrix("_MapUvs", MapUvs.worldToLocalMatrix);
        Shader.SetGlobalColor("_SideColor", BackgroundColor);
        SkyMat.SetColor("_Tint", BackgroundColor);
    }

    private Vector3 GetProvincePosition(int row, int ascendingColumn)
    {
        Vector2 ascendingOffset = new Vector2(1, -1.73f).normalized * ascendingColumn;
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
        if(index < 0 || index >= Tiles.Length)
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

    internal void TestConnect(Tile tile)
    {
        tile.Province = _testProvince;
    }
}

public class Province
{
    public HashSet<Tile> Tiles;

    public Province(params Tile[] tiles)
    {
        Tiles = new HashSet<Tile>(tiles);
    }
}


public class Tile : MonoBehaviour
{
    private bool _provincesNeedUpdate;
    private Province _province;
    public Province Province
    {
        get { return _province; }
        set
        {
            if(value != _province)
            {
                SetProvince(value);
            }
        }
    }

    private void SetProvince(Province newProvince)
    {
        if(_province != null)
        {
            _province.Tiles.Remove(this);
        }
        newProvince.Tiles.Add(this);
        _province = newProvince;
        _provincesNeedUpdate = true;
        foreach (Tile tile in Neighbors)
        {
            tile._provincesNeedUpdate = true;
        }
    }

    public Worldmap Map;

    public int Row;
    public int AscendingColumn;

    public Tile PositiveRow;
    public Tile NegativeRow;
    public Tile PositiveAscending;
    public Tile NegativeAscending;
    public Tile PositiveDescending;
    public Tile NegativeDescending;

    public IEnumerable<Tile> Neighbors
    {
        get
        {
            yield return PositiveRow;
            yield return NegativeRow;
            yield return PositiveAscending;
            yield return NegativeAscending;
            yield return PositiveDescending;
            yield return NegativeDescending;
        }
    }

    public bool ConnectionTest;

    private Material _mat;

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        _mat = renderer.material;
    }

    private void Update()
    {
        if(ConnectionTest)
        {
            Map.TestConnect(this);
            ConnectionTest = false;
        }
        if(_provincesNeedUpdate)
        {
            _provincesNeedUpdate = false;
            UpdateConnections();
        }
    }

    public Tile GetOffset(int rowOffset, int ascendingColumnOffset)
    {
        return Map.GetTile(Row + rowOffset, AscendingColumn + ascendingColumnOffset);
    }

    public void EstablishNeighbors()
    {
        PositiveRow = GetOffset(1, 0);
        NegativeRow = GetOffset(-1, 0);
        PositiveAscending = GetOffset(0, 1);
        NegativeAscending = GetOffset(0, -1);
        PositiveDescending = GetOffset(-1, 1);
        NegativeDescending = GetOffset(1, -1);
    }

    public void UpdateConnections()
    {
        _mat.SetFloat("_PositiveRowConnected", PositiveRow.Province == Province ? 1 : 0);
        _mat.SetFloat("_NegativeRowConnected", NegativeRow.Province == Province ? 1 : 0);
        _mat.SetFloat("_PositiveAscendingConnected", PositiveAscending.Province == Province ? 1 : 0);
        _mat.SetFloat("_NegativeAscendingConnected", NegativeAscending.Province == Province ? 1 : 0);
        _mat.SetFloat("_PositiveDescendingConnected", PositiveDescending.Province == Province ? 1 : 0);
        _mat.SetFloat("_NegativeDescendingConnected", NegativeDescending.Province == Province ? 1 : 0);
    }
}