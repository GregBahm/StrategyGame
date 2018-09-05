public class Tile
{
    public int Row { get; }
    public int AscendingColumn { get; }

    public Tile(int row, int ascendingColumn)
    {
        Row = row;
        AscendingColumn = ascendingColumn;
    }
}
