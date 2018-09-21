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
}