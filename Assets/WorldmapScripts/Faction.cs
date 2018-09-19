using UnityEngine;

public class Faction
{
    public string Name { get; }
    public Color Color { get; }

    public Faction(string name, Color color)
    {
        Name = name;
        Color = color;
    }
}
