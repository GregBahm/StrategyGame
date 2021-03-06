﻿using System;
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

    internal static IEnumerable<PlayerSetup> CreateFromMapDefinition(MapTilesSetup mapSetup)
    {
        List<PlayerSetup> ret = new List<PlayerSetup>();
        int index = 0;
        foreach (MapTileSetup item in mapSetup.Tiles.Where(item => item.IsStartPosition))
        {
            index++;
            string name = "Player " + index;
            Color playerColor = GetPlayerColor();
            PlayerSetup retItem = new PlayerSetup(name, playerColor, item.Row, item.Column);
            ret.Add(retItem);
        }
        return ret;
    }

    private static Color GetPlayerColor()
    {
        Vector3 vect = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
        vect.Normalize();
        return new Color(vect.x, vect.y, vect.z);
    }
}