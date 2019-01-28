using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;

public class MapDefinition
{
    private ReadOnlyDictionary<string, MapTileDefinition> _tiles;
    public IEnumerable<MapTileDefinition> Tiles { get { return _tiles.Values; } }
    
    public MapDefinition(TextAsset mapDefinition)
    {
        IEnumerable<MapTileDefinition> baseTiles = LoadMapmakerTiles(mapDefinition).ToArray();
        IEnumerable<MapTileDefinition> allTiles = GetAllTiles(baseTiles);
        Dictionary<string, MapTileDefinition> table = allTiles.ToDictionary(item => GetKey(item.Row, item.Column), item => item);
        _tiles = new ReadOnlyDictionary<string, MapTileDefinition>(table);
    }

    private string GetKey(int row, int column)
    {
        return row.ToString() + " " + column.ToString();
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

    internal bool ContainsDefinitionFor(int row, int column)
    {
        string key = GetKey(row, column);
        return _tiles.ContainsKey(key);
    }
    internal MapTileDefinition GetDefinitionFor(int row, int column)
    {
        string key = GetKey(row, column);
        return _tiles[key];
    }
}
