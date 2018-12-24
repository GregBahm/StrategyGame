using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ProvinceNeighborsTable
{
    private readonly ReadOnlyDictionary<Province, HashSet<Province>> _neighborsDictionary;

    public ProvinceNeighborsTable(GameState state)
    {
        Dictionary<Province, HashSet<Province>> dictionary = new Dictionary<Province, HashSet<Province>>();
        foreach (ProvinceState province in state.Provinces)
        {
            HashSet<Province> neighbors = GetNeighborProvinces(state, province);
            dictionary.Add(province.Identifier, neighbors);
        }
        _neighborsDictionary = new ReadOnlyDictionary<Province, HashSet<Province>>(dictionary);
    }

    public HashSet<Province> GetNeighborsFor(Province province)
    {
        return _neighborsDictionary[province];
    }

    private HashSet<Province> GetNeighborProvinces(GameState state, ProvinceState province)
    {
        HashSet<Tile> neighborTiles = new HashSet<Tile>();
        foreach (Tile item in province.Tiles)
        {
            neighborTiles.UnionWith(item.Neighbors);
        }
        HashSet<Province> provinces = new HashSet<Province>();
        foreach (Tile item in neighborTiles)
        {
            Province tileProvince = state.GetTilesProvince(item).Identifier;
            provinces.Add(tileProvince);
        }
        provinces.Remove(province.Identifier);
        return provinces;
    }
}