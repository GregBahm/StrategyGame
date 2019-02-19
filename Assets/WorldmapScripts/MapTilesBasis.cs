using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Collections.ObjectModel;

public class MapTilesBasis
{
    private ReadOnlyDictionary<string, MapTileBasis> _tiles;
    public IEnumerable<MapTileBasis> Tiles { get { return _tiles.Values; } }
    
    public MapTilesBasis(TextAsset mapDefinition)
    {
        IEnumerable<MapTileBasis> baseTiles = LoadMapmakerTiles(mapDefinition).ToArray();
        IEnumerable<MapTileBasis> allTiles = GetAllTiles(baseTiles);
        Dictionary<string, MapTileBasis> table = allTiles.ToDictionary(item => GetKey(item.Row, item.Column), item => item);
        _tiles = new ReadOnlyDictionary<string, MapTileBasis>(table);
    }

    public bool ContainsDefinitionFor(int row, int column)
    {
        string key = GetKey(row, column);
        return _tiles.ContainsKey(key);
    }
    public MapTileBasis GetDefinitionFor(int row, int column)
    {
        string key = GetKey(row, column);
        return _tiles[key];
    }

    private string GetKey(int row, int column)
    {
        return row.ToString() + " " + column.ToString();
    }

    private IEnumerable<MapTileBasis> GetAllTiles(IEnumerable<MapTileBasis> baseTiles)
    {
        List<MapTileBasis> ret = new List<MapTileBasis>();
        ret.Add(new MapTileBasis(0, 0, false, false)); // The tile at the center of the map
        foreach (MapTileBasis item in baseTiles)
        {
            ret.AddRange(GetMirroredTiles(item));
        }
        return ret;
    }

    private IEnumerable<MapTileBasis> GetMirroredTiles(MapTileBasis item)
    {
        MapTileBasis rotationA = GetFirstRotatedTile(item);
        MapTileBasis rotationB = GetSecondRotatedTile(item);
        yield return item;
        yield return rotationA;
        yield return rotationB;
        yield return GetFlippedTile(item);
        yield return GetFlippedTile(rotationA);
        yield return GetFlippedTile(rotationB);
    }

    private MapTileBasis GetSecondRotatedTile(MapTileBasis item)
    {
        int ring = item.Row + item.Column;
        int offset = item.Row;
        int row = ring - offset;
        int column = -ring;
        return new MapTileBasis(row, column, item.IsImpassable, item.IsStartPosition);
    }

    private MapTileBasis GetFirstRotatedTile(MapTileBasis item)
    {
        int ring = item.Row + item.Column;
        int offset = item.Row;
        int row = ring;
        int column = -offset;
        return new MapTileBasis(row, column, item.IsImpassable, item.IsStartPosition);
    }

    private MapTileBasis GetFlippedTile(MapTileBasis item)
    {
        return new MapTileBasis(-item.Row, -item.Column, item.IsImpassable, item.IsStartPosition);
    }

    private IEnumerable<MapTileBasis> LoadMapmakerTiles(TextAsset mapDefinition)
    {
        string[] lines = mapDefinition.text.Split('\n');
        IEnumerable<MapTileBasis> allItems = lines.Where(line => !String.IsNullOrWhiteSpace(line)).Select(line => LoadMapmakerTile(line));
        return allItems.Where(item => !item.IsImpassable);
    }

    private MapTileBasis LoadMapmakerTile(string line)
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

        return new MapTileBasis(row, column, isImpassable, isStartPosition);
    }
}
