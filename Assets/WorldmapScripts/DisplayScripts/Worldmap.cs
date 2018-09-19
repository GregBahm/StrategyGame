using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Worldmap
{
    public TileDisplay[] Tiles { get; }
    public int Rows { get; }
    public int Columns { get; }
    public int TilesCount { get { return Rows * Columns; } }

    public readonly Vector2 AscendingTileOffset = new Vector2(1, -1.73f).normalized;
    
    public Worldmap(GameObject tilePrefab, int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        Tiles = MakeTiles(tilePrefab);
        InitializeTiles();
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

    private TileDisplay[] MakeTiles(GameObject tilePrefab)
    {
        TileDisplay[] ret = new TileDisplay[TilesCount];
        for (int row = 0; row < Rows; row++)
        {
            for (int ascendingColumn = 0; ascendingColumn < Columns; ascendingColumn++)
            {
                int index = (row * Columns) + ascendingColumn;
                ret[index] = CreateTile(row, ascendingColumn, tilePrefab);
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

    private TileDisplay CreateTile(int row, int ascendingColumn, GameObject tilePrefab)
    {
        int descendingColumn = row + ascendingColumn;
        string providenceName = string.Format("Providence {0} {1} {2}", row, ascendingColumn, descendingColumn);
        GameObject obj = GameObject.Instantiate(tilePrefab);
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
