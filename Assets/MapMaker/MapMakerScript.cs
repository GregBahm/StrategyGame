using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMakerScript : MonoBehaviour
{
    public GameObject TilePrefab;
    public int BaseExtent;
    private RingSideBlueprint[] _ringSideBlueprints;

    private void Start()
    {
        _ringSideBlueprints = CreateRingSideBlueprints();
        List<TileBlueprint> blueprints = GetBlueprints();
        foreach (TileBlueprint item in blueprints)
        {
            item.CreateGameobject(TilePrefab);
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

    private List<TileBlueprint> GetBlueprints()
    {
        List<TileBlueprint> ret = new List<TileBlueprint>();
        ret.Add(new TileBlueprint(false, 0, 0, 0));
        for (int i = 1; i < BaseExtent; i++)
        {
            ret.AddRange(MakeRing(i));
        }
        return ret;
    }

    private IEnumerable<TileBlueprint> MakeRing(int ring)
    {
        List<TileBlueprint> ret = new List<TileBlueprint>();
        for (int i = 0; i < ring; i++)
        {
            foreach (RingSideBlueprint blueprint in _ringSideBlueprints)
            {
                TileBlueprint retItem = DoRingSide(blueprint, ring, i);
                ret.Add(retItem);
            }
        }
        return ret;
    }

    private TileBlueprint DoRingSide(RingSideBlueprint blueprint, int ring, int ringSideIndex)
    {
        int startRow = blueprint.BaseRowMultiplier * ring;
        int startColumn = blueprint.BaseColumnMultiplier * ring;

        int rowOffset = blueprint.RowOffsetIncrement * ringSideIndex;
        int columnOffset = blueprint.ColumnOffsetIncrement * ringSideIndex;

        int finalRow = startRow + rowOffset;
        int finalColumn = startColumn + columnOffset;
        return new TileBlueprint(blueprint.IsMasterSide, ring, finalRow, finalColumn);
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
    
    class TileRotationSet
    {
        public TileBlueprint BaseTile { get; }
        public IEnumerable<TileBlueprint> MirroredTiles { get; }
    }

    class TileBlueprint
    {
        public bool IsMasterTile { get; }
        public Material TileMat { get; set; }
        public int Ring { get; }
        public int Row { get; }
        public int Column { get; }
        public TileBlueprint(bool isMaster,
            int ring, 
            int row, 
            int column)
        {
            IsMasterTile = isMaster;
            Ring = ring;
            Row = row;
            Column = column;
        }

        internal void CreateGameobject(GameObject tilePrefab)
        {
            GameObject obj = Instantiate(tilePrefab);
            obj.name = "Ring " + Ring + " row:" + Row + " column:" + Column;
            obj.transform.position = GetProvincePosition(Row, Column);
        }
        private Vector3 GetProvincePosition(int row, int ascendingColumn)
        {
            Vector2 ascendingOffset = MapDisplay.AscendingTileOffset * ascendingColumn;
            Vector2 offset = ascendingOffset + new Vector2(row, 0);
            offset *= 2;
            return new Vector3(offset.x, 0, offset.y);
        }
    }
}
