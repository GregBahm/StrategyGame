using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TileNeighbors : IEnumerable<Tile>
{
    public Tile NeighborA { get; }
    public Tile NeighborB { get; }
    public Tile NeighborC { get; }
    public Tile NeighborD { get; }
    public Tile NeighborE { get; }
    public Tile NeighborF { get; }

    private IEnumerable<Tile> _neighbors;

    public TileNeighbors(Tile neighborA, Tile neighborB, Tile neighborC, Tile neighborD, Tile neighborE, Tile neighborF)
    {
        NeighborA = neighborA;
        NeighborB = neighborB;
        NeighborC = neighborC;
        NeighborD = neighborD;
        NeighborE = neighborE;
        NeighborF = neighborF;
        _neighbors = new List<Tile>() { NeighborA, NeighborB, NeighborC, NeighborD, NeighborE, NeighborF }.Where(item => item != null).ToList();
    }

    public IEnumerator<Tile> GetEnumerator()
    {
        return _neighbors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
