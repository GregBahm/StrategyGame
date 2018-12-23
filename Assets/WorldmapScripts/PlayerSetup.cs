using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSetup
{
    public Faction Faction { get; }
    public int StartRow { get; }
    public int StartColumn { get; }

    public PlayerSetup(string name, Color color, int startRow, int startColumn)
    {
        Faction = new Faction(name, color);
        StartRow = startRow;
        StartColumn = startColumn;
    }

    internal static IEnumerable<PlayerSetup> CreateFromMapDefinition(MapDefinition mapDefinition)
    {
        List<PlayerSetup> ret = new List<PlayerSetup>();
        int index = 0;
        foreach (MapTileDefinition item in mapDefinition.Tiles.Where(item => item.IsStartPosition))
        {
            index++;
            string name = "Player " + index;
            Color randomColor = new Color(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
            PlayerSetup retItem = new PlayerSetup(name, randomColor, item.Row, item.Column);
            ret.Add(retItem);
        }
        return ret;
    }
}