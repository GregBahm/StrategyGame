using UnityEngine;

public class Faction
{
    public bool IsLocalPlayer { get; }
    public Color Color { get; }

    public Faction(bool isLocalPlayer, Color color)
    {
        IsLocalPlayer = isLocalPlayer;
        Color = color;
    }
}
