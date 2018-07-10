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

    private Province _testProvince;

    private void Start()
    {
        _rows = 20;
        _columns = 20;
        Tiles = MakeTiles();
        _testProvince = new Province();
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
                ret[index] = CreateTiles(row, ascendingColumn);
            }
        }
        return ret;
    }

    public Tile GetTile(int row, int ascendingColumn)
    {
        int index = (row * _columns) + ascendingColumn;
        return Tiles[index];
    }

    private Tile CreateTiles(int row, int ascendingColumn)
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
        _testProvince.AddTlle(tile);
    }
}

public class Province
{
    public HashSet<Tile> Tiles;

    public Province(params Tile[] tiles)
    {
        Tiles = new HashSet<Tile>(tiles);
    }

    public void AddTlle(Tile tile)
    {
        Connect(tile, 1, 0, item => item.NegativeRowConnected = true, item => item.PositiveRowConnected = true);
        Connect(tile, -1, 0, item => item.PositiveRowConnected = true, item => item.NegativeRowConnected = true);

        Connect(tile, 0, 1, item => item.NegativeAscendingConnected = true, item => item.PositiveAscendingConnected = true);
        Connect(tile, 0, -1, item => item.PositiveAscendingConnected = true, item => item.NegativeAscendingConnected = true);

        Connect(tile, 1, 1, item => item.NegativeDescendingConnected = true, item => item.PositiveDescendingConnected = true);
        Connect(tile, 1, -1, item => item.PositiveDescendingConnected = true, item => item.NegativeDescendingConnected = true);

        Tiles.Add(tile);
    }

    private void Connect(Tile tile, int rowOffset, int columnOffset, Action<Tile> fromSetter, Action<Tile> toSetter)
    {
        Tile potentialTile = tile.GetOffset(rowOffset, columnOffset);
        if(Tiles.Contains(potentialTile))
        {
            fromSetter(potentialTile);
            toSetter(tile);
        }
    }
}


public class Tile : MonoBehaviour
{
    public Worldmap Map;

    public int Row;
    public int AscendingColumn;

    public bool PositiveRowConnected;
    public bool NegativeRowConnected;
    public bool PositiveAscendingConnected;
    public bool NegativeAscendingConnected;
    public bool PositiveDescendingConnected;
    public bool NegativeDescendingConnected;

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

        _mat.SetFloat("_PositiveRowConnected", PositiveRowConnected ? 1 : 0);
        _mat.SetFloat("_NegativeRowConnected", NegativeRowConnected ? 1 : 0);
        _mat.SetFloat("_PositiveAscendingConnected", PositiveAscendingConnected ? 1 : 0);
        _mat.SetFloat("_NegativeAscendingConnected", NegativeAscendingConnected ? 1 : 0);
        _mat.SetFloat("_PositiveDescendingConnected", PositiveDescendingConnected ? 1 : 0);
        _mat.SetFloat("_NegativeDescendingConnected", NegativeDescendingConnected ? 1 : 0);
    }

    public Tile GetOffset(int rowOffset, int ascendingColumnOffset)
    {
        return Map.GetTile(Row + rowOffset, AscendingColumn + ascendingColumnOffset);
    }
}