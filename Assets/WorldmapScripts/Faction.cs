using System;
using UnityEngine;

public class Faction
{
    /// <summary>
    /// The faction for unowned tiles
    /// </summary>
    public static Faction Independent { get; } = new Faction("Independent", Color.white);

    public string Name { get; }
    public Color Color { get; }

    public Faction(string name, Color color)
    {
        Name = name;
        Color = color;
    }
}
