using UnityEngine;

public class MapTileSetup
{
    public int BufferIndex { get; }

    public Vector2 CenterPoint { get; }

    public int Row { get; }

    public int Column { get; }

    public bool IsStartPosition { get; }

    public MapTileSetup(int row, int column, Vector2 centerPoint, int bufferIndex, bool isStartPosition)
    {
        Row = row;
        Column = column;
        CenterPoint = centerPoint;
        BufferIndex = bufferIndex;
        IsStartPosition = isStartPosition;
    }
}