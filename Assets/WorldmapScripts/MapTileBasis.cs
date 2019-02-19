public class MapTileBasis
{
    public int Row { get; }
    public int Column { get; }
    public bool IsImpassable { get; }
    public bool IsStartPosition { get; }

    public MapTileBasis(int row, int column, bool isImpassable, bool isStartPosition)
    {
        Row = row;
        Column = column;
        IsImpassable = isImpassable;
        IsStartPosition = isStartPosition;
    }
}