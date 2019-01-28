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
    private readonly ReadOnlyDictionary<Tile, IEnumerable<Tile>> _neighbors;
    
    public int TilesCount { get { return _tiles.Count; } }

    public Map(MapDefinition mapDefinition)
    {
        _maxRow = mapDefinition.Tiles.Max(item => Mathf.Abs(item.Row)) + 1;
        _maxColumn = mapDefinition.Tiles.Max(item => Mathf.Abs(item.Column)) + 1;
        _tiles = MakeTiles(mapDefinition.Tiles);
        _neighbors = GetNeighborsDictionary();
    }

    private ReadOnlyDictionary<Tile, IEnumerable<Tile>> GetNeighborsDictionary()
    {
        Dictionary<Tile, IEnumerable<Tile>> ret = new Dictionary<Tile, IEnumerable<Tile>>();
        foreach (Tile tile in _tiles.Values)
        {
            List<Tile> neighbors = GetNeighborsList(tile).Where(item => item != null).ToList();
            ret.Add(tile, neighbors);
        }
        return new ReadOnlyDictionary<Tile, IEnumerable<Tile>>(ret);
    }

    private IEnumerable<Tile> GetNeighborsList(Tile tile)
    {
        yield return TryGetTile(tile, 1, 0);
        yield return TryGetTile(tile, 0, 1);
        yield return TryGetTile(tile, -1, 0);
        yield return TryGetTile(tile, 0, -1);
        yield return TryGetTile(tile, 1, -1);
        yield return TryGetTile(tile, -1, 1);
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

    internal IEnumerable<Tile> GetNeighborsFor(Tile tile)
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

    private ReadOnlyDictionary<int, Tile> MakeTiles(IEnumerable<MapTileDefinition> tileDefinitions)
    {
        Dictionary<int, Tile> ret = new Dictionary<int, Tile>();
        ret.Add(GetKey(0, 0), new Tile(0, 0, this));
        foreach (MapTileDefinition item in tileDefinitions)
        {
            int key = GetKey(item.Row, item.Column);
            Tile tile = new Tile(item.Row, item.Column, this);
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
