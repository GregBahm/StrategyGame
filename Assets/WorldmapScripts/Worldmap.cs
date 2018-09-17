using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Worldmap
{
    public TileDisplay HighlitTile { get; private set; }

    public TileDisplay[] Tiles { get; }
    //public Color BackgroundColor;
    //public Material SkyMat;
    public int Rows { get; } = 20;
    public int Columns { get; } = 20;
    public int TilesCount { get { return Rows * Columns; } }
    //public float HighlightDecaySpeed;

    public readonly Vector2 AscendingTileOffset = new Vector2(1, -1.73f).normalized;

    private readonly MainGameManager _mainManager;
    private readonly TileMouseInteraction _tileManager;

    public Worldmap(MainGameManager mainManager)
    {
        _mainManager = mainManager;
        Tiles = MakeTiles();
        InitializeTiles();
        _tileManager = new TileMouseInteraction(this);
    }

    private void InitializeTiles()
    {
        foreach (TileDisplay tile in Tiles)
        {
            tile.SetNeighbors(this);
        }
        foreach (TileDisplay tile in Tiles)
        {
            tile.SetCollisionCluster();
        }
    }

    private void Update()
    {
        HighlitTile = _tileManager.GetTileUnderMouse();
        //Shader.SetGlobalFloat("_TileMargin", TileMargin);
        //Shader.SetGlobalMatrix("_MapUvs", MapUvs.worldToLocalMatrix);
        //Shader.SetGlobalColor("_SideColor", BackgroundColor);
        //SkyMat.SetColor("_Tint", BackgroundColor);
    }

    private TileDisplay[] MakeTiles()
    {
        TileDisplay[] ret = new TileDisplay[TilesCount];
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
    
    public TileDisplay GetTile(int row, int ascendingColumn)
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

    private TileDisplay CreateTile(int row, int ascendingColumn)
    {
        int descendingColumn = row + ascendingColumn;
        string providenceName = string.Format("Providence {0} {1} {2}", row, ascendingColumn, descendingColumn);
        GameObject obj = GameObject.Instantiate(_mainManager.TilePrefab);
        obj.name = providenceName;

        Tile tile = new Tile(row, ascendingColumn);
        TileDisplay ret = new TileDisplay(tile, this, obj); 
        obj.transform.position = GetProvincePosition(row, ascendingColumn);
        return ret;
    }


    public Vector3 GetProvincePosition(int row, int ascendingColumn)
    {
        Vector2 ascendingOffset = AscendingTileOffset * ascendingColumn;
        Vector2 offset = ascendingOffset + new Vector2(row, 0);
        offset *= 2;
        return new Vector3(offset.x, 0, offset.y);
    }
}
