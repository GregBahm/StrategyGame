using System;
using System.Collections.Generic;

public class Tile
{
    private Map _map;
    public int Row { get; }
    public int AscendingColumn { get; }
    public IEnumerable<Tile> Neighbors { get { return _map.GetNeighborsFor(this); } }
    
    public Tile(int row, int ascendingColumn, Map map)
    {
        _map = map;
        Row = row;
        AscendingColumn = ascendingColumn;
    }

    /// <summary>
    /// Returns the tile if it is within the bounds of the map. Otherwise returns null.
    /// </summary>
    public Tile GetOffset(int rowOffset, int ascendingColumnOffset)
    {
        int newRow = Row + rowOffset;
        int newColumn = AscendingColumn + ascendingColumnOffset;
        return _map.GetTile(newRow, newColumn);
    }
}
