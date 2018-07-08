using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worldmap : MonoBehaviour
{
    public Tile[] Tiles;
    public GameObject TilePrefab;
    public Transform MapUvs;
    [Range(0, 1)]
    public float TileMargin;
    public Color BackgroundColor;
    public Material SkyMat;

    private void Start()
    {
        Tiles = MakeTiles(20, 20);
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

    private Tile[] MakeTiles(int rows, int columns)
    {
        Tile[] ret = new Tile[rows * columns];
        for (int row = 0; row < rows; row++)
        {
            for (int ascendingColumn = 0; ascendingColumn < columns; ascendingColumn++)
            {
                int index = (row * columns) + ascendingColumn;
                ret[index] = CreateTiles(row, ascendingColumn - (row / 2));
            }
        }
        return ret;
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
        ret.DescendingColumn = descendingColumn;
        obj.transform.position = GetProvincePosition(row, ascendingColumn);
        return ret;
    }
}

public class TileConnection
{
    Tile ProvinceA;
    Tile ProvinceB;
}

public class Tile : MonoBehaviour
{
    public Worldmap Map;

    public int Row;
    public int AscendingColumn;
    public int DescendingColumn;

    public bool PositiveRowConnected;
    public bool NegativeRowConnected;
    public bool PositiveAscendingConnected;
    public bool NegativeAscendingConnected;
    public bool PositiveDescendingConnected;
    public bool NegativeDescendingConnected;

    private Material _mat;

    private void Start()
    {
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        _mat = renderer.material;
    }

    private void Update()
    {
        _mat.SetFloat("_PositiveRowConnected", PositiveRowConnected ? 1 : 0);
        _mat.SetFloat("_NegativeRowConnected", NegativeRowConnected ? 1 : 0);
        _mat.SetFloat("_PositiveAscendingConnected", PositiveAscendingConnected ? 1 : 0);
        _mat.SetFloat("_NegativeAscendingConnected", NegativeAscendingConnected ? 1 : 0);
        _mat.SetFloat("_PositiveDescendingConnected", PositiveDescendingConnected ? 1 : 0);
        _mat.SetFloat("_NegativeDescendingConnected", NegativeDescendingConnected ? 1 : 0);
    }

    public Tile GetOffset(int rowOffset, int ascendingColumnOffset, int decendingColumnOffset)
    {
        throw new NotImplementedException();
    }
}