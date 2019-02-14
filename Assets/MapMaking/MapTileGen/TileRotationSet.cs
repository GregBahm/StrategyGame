using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileRotationSet
{
    public MapMakerScript Main { get; }
    private IEnumerable<Material> _allMaterials;
    public MapmakerTile MasterTile { get; }
    public IEnumerable<MapmakerTile> MirroredTiles { get; }
    public TileRotationSet(MapMakerScript main, MapmakerTile masterTile, IEnumerable<MapmakerTile> mirroredTiles)
    {
        Main = main;
        MasterTile = masterTile;
        MirroredTiles = mirroredTiles;
    }

    public void SetAllMaterials()
    {
        List<Material> materials = new List<Material>();
        materials.Add(MasterTile.TileMat);
        materials.AddRange(MirroredTiles.Select(item => item.TileMat));
        _allMaterials = materials;
    }

    public void UpdateMaterials()
    {
        bool isStartPosition = MasterTile.TileBehavior.IsStartPosition;
        bool isImpassable = MasterTile.TileBehavior.IsImpassable;

        MasterTile.TileMat.SetFloat("_IsMaster", 1);
        foreach (Material mat in _allMaterials)
        {
            mat.SetFloat("_IsStartPosition", isStartPosition ? 1 : 0);
            mat.SetFloat("_IsImpassable", isImpassable ? 1 : 0);
        }
    }
}