using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Map : IEnumerable<Tile>
{
    private readonly List<Tile> _tiles;
    public Tile this[int tileIndex] { get { return _tiles[tileIndex]; } }

    public int Rows { get; }
    public int Columns { get; }
    public int TilesCount { get { return Rows * Columns; } }

    public Map(int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        _tiles = MakeTiles();
    }

    private List<Tile> MakeTiles()
    {
        List<Tile> ret = new List<Tile>(TilesCount);
        for (int row = 0; row < Rows; row++)
        {
            for (int ascendingColumn = 0; ascendingColumn < Columns; ascendingColumn++)
            {
                ret.Add(new Tile(row, ascendingColumn));
            }
        }
        return ret;
    }
    
    private static int MathMod(int value, int modolus)
    {
        return (Math.Abs(value * modolus) + value) % modolus;
    }

    public Tile GetTile(int row, int ascendingColumn)
    {
        int modRow = MathMod(row, Rows);
        int modColumn = MathMod(ascendingColumn, Columns);
        int index = (modRow * Columns) + modColumn;
        if (index < 0 || index >= TilesCount)
        {
            throw new Exception("Bad index (" + index + ")");
        }
        return _tiles[index];
    }

    public IEnumerator<Tile> GetEnumerator()
    {
        return _tiles.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _tiles.GetEnumerator();
    }
}
