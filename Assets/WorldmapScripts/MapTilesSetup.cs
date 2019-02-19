using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml;
using UnityEngine;

public class MapTilesSetup
{
    public ReadOnlyCollection<MapTileSetup> Tiles { get; }

    public MapTilesSetup(MapTilesBasis basis, string setupSaveData)
    {
        IEnumerable<SetupData> setupData = LoadSetupData(setupSaveData);
        Tiles = GetTiles(basis, setupData).ToList().AsReadOnly();
    }

    private IEnumerable<MapTileSetup> GetTiles(MapTilesBasis basis, IEnumerable<SetupData> setupData)
    {
        Dictionary<string, SetupData> table = setupData.ToDictionary(item => GetKey(item.Row, item.Column), item => item);
        foreach (MapTileBasis item in basis.Tiles.Where(item => !item.IsImpassable))
        {
            string key = GetKey(item.Row, item.Column);
            SetupData addedData = table[key];
            yield return new MapTileSetup(item.Row, item.Column, addedData.Center, addedData.BufferIndex, item.IsStartPosition);
        }
    }

    private static string GetKey(int x, int y)
    {
        return x + " " + y;
    }

    private IEnumerable<SetupData> LoadSetupData(string setupSaveData)
    {
        string[] data = setupSaveData.Split('\n');
        foreach (string line in data)
        {
            yield return LoadSetupDataLine(line);
        }
    }

    private SetupData LoadSetupDataLine(string line)
    {
        string[] split = line.Split(' ');
        int row = Convert.ToInt32(split[0]);
        int column = Convert.ToInt32(split[1]);
        int index = Convert.ToInt32(split[2]);
        float centerX = Convert.ToSingle(split[3]);
        float centerY = Convert.ToSingle(split[4]);
        return new SetupData(row, column, index, new Vector2(centerX, centerY));
    }

    private class SetupData
    {
        public int Row { get; }
        public int Column { get; }

        public int BufferIndex { get; }
        public Vector2 Center { get; }

        public SetupData(int row, int column, int bufferIndex, Vector2 center)
        {
            Row = row;
            Column = column;
            BufferIndex = bufferIndex;
            Center = center;
        }
    }
}
