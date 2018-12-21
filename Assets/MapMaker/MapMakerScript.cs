using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class MapMakerScript : MonoBehaviour
{
    public GameObject TilePrefab;
    public int BaseExtent;
    private RingSideBlueprint[] _ringSideBlueprints;
    private List<TileRotationSet> _rotationSets;

    public bool SaveMap;
    public bool LoadMap;
    public bool ForceUpdateShaders;
    private const string MapSaveFile = @"C:\Users\Lisa\Documents\StrategyGame\MapDefinition.txt";

    public int StartingLocations;

    private void Start()
    {
        _ringSideBlueprints = CreateRingSideBlueprints();
        _rotationSets = GetRotationSets();

        foreach (TileRotationSet rotationSet in _rotationSets)
        {
            rotationSet.MasterTile.CreateGameobject(TilePrefab);
            foreach (MapmakerTile tile in rotationSet.MirroredTiles)
            {
                tile.CreateGameobject(TilePrefab);
            }
            rotationSet.SetAllMaterials();
            rotationSet.UpdateMaterials();
            rotationSet.MasterTile.TileBehavior.RotationSet = rotationSet;
        }
    }

    public void UpdateStartingLocations()
    {
        StartingLocations = _rotationSets.Count(item => item.MasterTile.TileBehavior.IsStartPosition) * 6;
    }

    private void Update()
    {
        if(SaveMap)
        {
            SaveMap = false;
            DoSaveMap();
        }
        if(LoadMap)
        {
            LoadMap = false;
            DoLoadMap();
        }
        if(ForceUpdateShaders)
        {
            ForceUpdateShaders = false;
            DoForceUpdateShaders();
        }
    }

    private void DoForceUpdateShaders()
    {
        foreach (TileRotationSet set in _rotationSets)
        {
            set.UpdateMaterials();
        }
    }

    private void DoSaveMap()
    {
        StringBuilder builder = new StringBuilder();
        foreach (TileRotationSet set in _rotationSets)
        {
            string line = set.MasterTile.GetSaveLine();
            builder.AppendLine(line);
        }
        File.WriteAllText(MapSaveFile, builder.ToString());
    }

    private void DoLoadMap()
    {
        string[] saveFile = File.ReadAllLines(MapSaveFile);
        Dictionary<string, string> lookupTable = new Dictionary<string, string>();
        foreach (string line in saveFile)
        {
            string[] split = line.Split(',');
            if(split.Length == 2) // Avoids last empty line
            {
                lookupTable.Add(split[0], split[1]);
            }
        }
        foreach (TileRotationSet set in _rotationSets)
        {
            string loadData = lookupTable[set.MasterTile.Key];
            set.MasterTile.LoadFromSave(loadData);
        }
    }

    private RingSideBlueprint[] CreateRingSideBlueprints()
    {
        RingSideBlueprint[] ret = new RingSideBlueprint[]
        {
            new RingSideBlueprint(1, 0, 0, -1, true),
            new RingSideBlueprint(-1, 1, 1, 0),
            new RingSideBlueprint(0, 1, 1, -1),
            new RingSideBlueprint(-1, 0, 0, 1),
            new RingSideBlueprint(1, -1, -1, 0),
            new RingSideBlueprint(0, -1, -1, 1)
        };
        return ret;
    }

    private List<TileRotationSet> GetRotationSets()
    {
        List<TileRotationSet> ret = new List<TileRotationSet>();
        for (int i = 1; i < BaseExtent; i++)
        {
            ret.AddRange(MakeRing(i));
        }
        return ret;
    }

    private IEnumerable<TileRotationSet> MakeRing(int ring)
    {
        List<TileRotationSet> ret = new List<TileRotationSet>();
        for (int i = 0; i < ring; i++)
        {
            MapmakerTile masterTile = null;
            List<MapmakerTile> tiles = new List<MapmakerTile>();
            foreach (RingSideBlueprint blueprint in _ringSideBlueprints)
            {
                MapmakerTile tile = DoRingSide(blueprint, ring, i);
                if(blueprint.IsMasterSide)
                {
                    masterTile = tile;
                }
                else
                {
                    tiles.Add(tile);
                }
            }
            TileRotationSet retItem = new TileRotationSet(this, masterTile, tiles);
            ret.Add(retItem);
        }
        return ret;
    }

    private MapmakerTile DoRingSide(RingSideBlueprint blueprint, int ring, int ringSideIndex)
    {
        int startRow = blueprint.BaseRowMultiplier * ring;
        int startColumn = blueprint.BaseColumnMultiplier * ring;

        int rowOffset = blueprint.RowOffsetIncrement * ringSideIndex;
        int columnOffset = blueprint.ColumnOffsetIncrement * ringSideIndex;

        int finalRow = startRow + rowOffset;
        int finalColumn = startColumn + columnOffset;
        return new MapmakerTile(blueprint.IsMasterSide, ring, finalRow, finalColumn);
    }

    class RingSideBlueprint
    {
        public int BaseRowMultiplier { get; }
        public int BaseColumnMultiplier { get; }
        public int RowOffsetIncrement { get; }
        public int ColumnOffsetIncrement { get; }
        public bool IsMasterSide { get; }

        public RingSideBlueprint(int baseRowMultiplier, 
            int baseColumnMultiplier,
            int rowOffsetIncrement,
            int columnOffsetIncrement,
            bool isMasterSide = false)
        {
            BaseRowMultiplier = baseRowMultiplier;
            BaseColumnMultiplier = baseColumnMultiplier;
            RowOffsetIncrement = rowOffsetIncrement;
            ColumnOffsetIncrement = columnOffsetIncrement;
            IsMasterSide = isMasterSide;
        }
    }
}