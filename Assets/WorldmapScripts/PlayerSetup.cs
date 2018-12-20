﻿using System.Collections.Generic;
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

    public static IEnumerable<PlayerSetup> GetTestSetups()
    {
        return new[]
        {
            new PlayerSetup("Player A", Color.blue, 0,0),
            new PlayerSetup("Player B", Color.red, 10, 10),
            new PlayerSetup("Player C", Color.gray, 19, 19),
        };
    }
}