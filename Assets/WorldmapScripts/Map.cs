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
    
    public int TilesCount { get { return _tiles.Count; } }

    public Map(MapDefinition mapDefinition)
    {
        _maxRow = mapDefinition.Tiles.Max(item => Mathf.Abs(item.Row)) + 1;
        _maxColumn = mapDefinition.Tiles.Max(item => Mathf.Abs(item.Column)) + 1;
        _tiles = MakeTiles(mapDefinition.Tiles);
    }

    private int GetKey(int row, int ascendingColumn)
    {
        // Remap rows and column numbers to positive space
        int offsetRow = row + _maxRow;
        int offsetColumn = ascendingColumn + _maxColumn;

        return offsetRow * (_maxColumn * 2) + offsetColumn;
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
            if(ret.ContainsKey(key))
            {
                Debug.Log("row " + item.Row + " column " + item.Column);
            }
            else
            {
                ret.Add(key, tile);
            }
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

public class MapDefinition
{
    public IEnumerable<MapTileDefinition> Tiles { get; }
    
    public MapDefinition(TextAsset mapDefinition)
    {
        IEnumerable<MapTileDefinition> baseTiles = LoadMapmakerTiles(mapDefinition).ToArray();
        Tiles = GetAllTiles(baseTiles);
    }

    private IEnumerable<MapTileDefinition> GetAllTiles(IEnumerable<MapTileDefinition> baseTiles)
    {
        List<MapTileDefinition> ret = new List<MapTileDefinition>();
        foreach (MapTileDefinition item in baseTiles)
        {
            ret.AddRange(GetMirroredTiles(item));
        }
        return ret;
    }

    private IEnumerable<MapTileDefinition> GetMirroredTiles(MapTileDefinition item)
    {
        MapTileDefinition rotationA = GetFirstRotatedTile(item);
        MapTileDefinition rotationB = GetSecondRotatedTile(item);
        yield return item;
        yield return rotationA;
        yield return rotationB;
        yield return GetFlippedTile(item);
        yield return GetFlippedTile(rotationA);
        yield return GetFlippedTile(rotationB);
    }

    private MapTileDefinition GetSecondRotatedTile(MapTileDefinition item)
    {
        int ring = item.Row + item.Column;
        int offset = item.Row;
        int row = ring - offset;
        int column = -ring;
        return new MapTileDefinition(row, column, item.IsImpassable, item.IsStartPosition);
    }

    private MapTileDefinition GetFirstRotatedTile(MapTileDefinition item)
    {
        int ring = item.Row + item.Column;
        int offset = item.Row;
        int row = ring;
        int column = -offset;
        return new MapTileDefinition(row, column, item.IsImpassable, item.IsStartPosition);
    }

    private MapTileDefinition GetFlippedTile(MapTileDefinition item)
    {
        return new MapTileDefinition(-item.Row, -item.Column, item.IsImpassable, item.IsStartPosition);
    }

    private IEnumerable<MapTileDefinition> LoadMapmakerTiles(TextAsset mapDefinition)
    {
        string[] lines = mapDefinition.text.Split('\n');
        IEnumerable<MapTileDefinition> allItems = lines.Where(line => !String.IsNullOrWhiteSpace(line)).Select(line => LoadMapmakerTile(line));
        return allItems.Where(item => !item.IsImpassable);
    }

    private MapTileDefinition LoadMapmakerTile(string line)
    {
        string[] firstSplit = line.Split(',');
        string[] locationSplit = firstSplit[0].Split(' ');
        string rowString = locationSplit[0];
        string columnString = locationSplit[1];
        int row = Convert.ToInt32(rowString);
        int column = Convert.ToInt32(columnString);

        string[] boolsSplit = firstSplit[1].Split(' ');
        string isImpassableString = boolsSplit[0];
        string isStartPositionString = boolsSplit[1];
        bool isImpassable = Convert.ToBoolean(isImpassableString);
        bool isStartPosition = Convert.ToBoolean(isStartPositionString);

        return new MapTileDefinition(row, column, isImpassable, isStartPosition);
    }
}
