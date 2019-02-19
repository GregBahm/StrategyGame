using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;

public class Map : IEnumerable<Tile>
{
    private readonly int _maxRow;

    private readonly int _maxColumn;
    private readonly ReadOnlyDictionary<int, Tile> _tiles;
    private readonly ReadOnlyDictionary<Tile, TileNeighbors> _neighbors;
    
    public int TilesCount { get { return _tiles.Count; } }

    public Map(MapTilesSetup mapSetup)
    {
        _maxRow = mapSetup.Tiles.Max(item => Mathf.Abs(item.Row)) + 1;
        _maxColumn = mapSetup.Tiles.Max(item => Mathf.Abs(item.Column)) + 1;
        _tiles = MakeTiles(mapSetup.Tiles);
        _neighbors = GetNeighborsDictionary();
    }

    private ReadOnlyDictionary<Tile, TileNeighbors> GetNeighborsDictionary()
    {
        Dictionary<Tile, TileNeighbors> ret = new Dictionary<Tile, TileNeighbors>();
        foreach (Tile tile in _tiles.Values)
        {
            TileNeighbors neighbors = GetNeighborsList(tile);
            ret.Add(tile, neighbors);
        }
        return new ReadOnlyDictionary<Tile, TileNeighbors>(ret);
    }

    private TileNeighbors GetNeighborsList(Tile tile)
    {
        return new TileNeighbors(
            TryGetTile(tile, 0, -1),
            TryGetTile(tile, -1, 0),
            TryGetTile(tile, -1, 1),
            TryGetTile(tile, 0, 1),
            TryGetTile(tile, 1, 0),
            TryGetTile(tile, 1, -1)
        );
    }

    private Tile TryGetTile(Tile tile, int rowOffset, int tileOffset)
    {
        if (GetIsWithinBounds(tile, rowOffset, tileOffset))
        {
            return tile.GetOffset(rowOffset, tileOffset);
        }
        return null;
    }

    private int GetKey(int row, int ascendingColumn)
    {
        // Remap rows and column numbers to positive space
        int offsetRow = row + _maxRow;
        int offsetColumn = ascendingColumn + _maxColumn;

        return offsetRow * (_maxColumn * 2) + offsetColumn;
    }

    internal TileNeighbors GetNeighborsFor(Tile tile)
    {
        return _neighbors[tile];
    }

    public bool GetIsWithinBounds(int row, int ascendingColumn)
    {
        int index = GetKey(row, ascendingColumn);
        return _tiles.ContainsKey(index);
    }
    public bool GetIsWithinBounds(Tile tile, int rowOffset, int columnOffset)
    {
        int row = tile.Row + rowOffset;
        int column = tile.AscendingColumn + columnOffset;
        return GetIsWithinBounds(row, column);
    }

    private ReadOnlyDictionary<int, Tile> MakeTiles(IEnumerable<MapTileSetup> tileDefinitions)
    {
        Dictionary<int, Tile> ret = new Dictionary<int, Tile>();
        foreach (MapTileSetup item in tileDefinitions)
        {
            int key = GetKey(item.Row, item.Column);
            Tile tile = new Tile(item.Row, item.Column, item.CenterPoint, item.BufferIndex, this);
            ret.Add(key, tile);
        }
        return new ReadOnlyDictionary<int, Tile>(ret); 
    }

    public Tile GetTile(int row, int ascendingColumn)
    {
        int index = GetKey(row, ascendingColumn);
        if (!_tiles.ContainsKey(index))
        {
            throw new Exception("No tile exists at " + row + " " + ascendingColumn);
        }
        return _tiles[index];
    }
    public IEnumerator<Tile> GetEnumerator()
    {
        return _tiles.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _tiles.GetEnumerator();
    }
}
