public class MapTileDefinition
{
    public int Row { get; }
    public int Column { get; }
    public bool IsImpassable { get; }
    public bool IsStartPosition { get; }

    public MapTileDefinition(int row, int column, bool isImpassable, bool isStartPosition)
    {
        Row = row;
        Column = column;
        IsImpassable = isImpassable;
        IsStartPosition = isStartPosition;
    }
}