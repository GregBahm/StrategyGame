using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BaseMapManager
{
    private readonly MainMapManager _main;
    public int MaxIndex { get; private set; } = 1;

    private readonly int _extents;
    private int _currentHexIndex;

    public ComputeBuffer CornersData { get; }
    private const int CornerPointsStride = sizeof(float) * 2 * 6;

    public ComputeBuffer NeighborsData { get; }
    private const int NeighborsDataStride = sizeof(uint) * 6;

    public ReadOnlyCollection<HexCenter> BaseHexs { get; }

    public struct Corners
    {
        public Vector2 CornerA;
        public Vector2 CornerB;
        public Vector2 CornerC;
        public Vector2 CornerD;
        public Vector2 CornerE;
        public Vector2 CornerF;
    }

    public struct NeighborIndices
    {
        public uint NeighborA;
        public uint NeighborB;
        public uint NeighborC;
        public uint NeighborD;
        public uint NeighborE;
        public uint NeighborF;
    }

    public BaseMapManager(MainMapManager main)
    {
        _main = main;
        _extents = main.MapDefinition.Tiles.Max(item => item.Row) + 2; // The "+ 2" is to add a dummy ring around the playable tiles
        List<HexCenter> hexes = GetHexCenterPoints().ToList();
        hexes.OrderBy(item => item.Index);
        BaseHexs = hexes.AsReadOnly();
        MaxIndex = BaseHexs.Count();
        MakeMap(BaseHexs);
        CornersData = GetCornerPoints();
        NeighborsData = GetNeihborsData();
    }

    public void Update()
    {
        _main.BaseMapMat.SetTexture("_MainTex", _main.BaseTexture);
        _main.BaseMapMat.SetFloat("_MaxIndex", MaxIndex);
        _main.BaseMapMat.SetBuffer("_CornersData", CornersData);
    }

    public void OnDestroy()
    {
        CornersData.Dispose();
        NeighborsData.Dispose();
    }

    private ComputeBuffer GetNeihborsData()
    {
        ComputeBuffer ret = new ComputeBuffer(MaxIndex, NeighborsDataStride);
        NeighborIndices[] data = new NeighborIndices[MaxIndex];
        Dictionary<string, HexCenter> indexTable = BaseHexs.ToDictionary(GetKey, item => item);
        for (int i = 0; i < MaxIndex; i++)
        {
            HexCenter hex = BaseHexs[i];
            NeighborIndices neighbors = GetNeihborsFor(hex, indexTable);
            data[hex.Index] = neighbors;
        }
        ret.SetData(data);
        return ret;
    }

    private uint GetOffsetHex(HexCenter hex, int rowOffset, int columnOffset, Dictionary<string, HexCenter> lookupTable)
    {
        int newRow = hex.Row + rowOffset;
        int newColumn = hex.Column + columnOffset;
        string key = GetKey(newRow, newColumn);
        if(lookupTable.ContainsKey(key))
        {
            HexCenter neighbor = lookupTable[key];
            return (uint)neighbor.Index;
        }
        return 0;
    }
    private string GetKey(HexCenter hex)
    {
        return GetKey(hex.Row, hex.Column);
    }
    private string GetKey(int row, int column)
    {
        return row + " " + column;
    }

    private NeighborIndices GetNeihborsFor(HexCenter hex, Dictionary<string, HexCenter> table)
    {
        return new NeighborIndices()
        {
            NeighborA = GetOffsetHex(hex, 0, -1, table),
            NeighborB = GetOffsetHex(hex, -1, 0, table),
            NeighborC = GetOffsetHex(hex, -1, 1, table),
            NeighborD = GetOffsetHex(hex, 0, 1, table),
            NeighborE = GetOffsetHex(hex, 1, 0, table),
            NeighborF = GetOffsetHex(hex, 1, -1, table),
        };
    }

    private ComputeBuffer GetCornerPoints()
    {
        ComputeBuffer ret = new ComputeBuffer(MaxIndex, CornerPointsStride);
        Corners[] data = new Corners[MaxIndex];
        foreach (HexCenter item in BaseHexs)
        {
            data[item.Index] = item.Corners;
        }
        ret.SetData(data);
        return ret;
    }

    private void MakeMap(IEnumerable<HexCenter> hexCenters)
    {
        HexTable table = new HexTable(hexCenters, _extents);
        for (int xIndex = 0; xIndex < _main.BaseTexture.width; xIndex++)
        {
            for (int yIndex = 0; yIndex < _main.BaseTexture.height; yIndex++)
            {
                HexCenter hexCenter = table.GetHexCenter(xIndex, yIndex, _main.BaseTexture.width, _main.BaseTexture.height);
                Color pixelColor = GetMapValue(hexCenter);
                _main.BaseTexture.SetPixel(xIndex, yIndex, pixelColor);
            }
        }
        _main.BaseTexture.Apply();
    }

    private Color GetMapValue(HexCenter hexCenter)
    {
        if(hexCenter == null || hexCenter.Index == 0)
        {
            return Color.black;
        }

        int red = hexCenter.Index % 255;
        float redVal = (float)red / 255;

        int green = hexCenter.Index / 255;
        float greenVal = (float)green / 255;

        return new Color(redVal, greenVal, 0);
    }

    private IEnumerable<HexCenter> GetHexCenterPoints()
    {
        RingSideBlueprint[] ringSideBlueprints = CreateRingSideBlueprints();
        yield return new HexCenter(new Vector2(.5f, .5f), RegisterNextHex(), 0, 0, _extents); // The hex in the center of the map
        for (int ring = 1; ring < _extents; ring++)
        {
            for (int i = 0; i < ring; i++)
            {
                foreach (RingSideBlueprint blueprint in ringSideBlueprints)
                {
                    yield return DoRingSide(blueprint, ring, i);
                }
            }
        }
    }

    private HexCenter DoRingSide(RingSideBlueprint blueprint, int ring, int ringSideIndex)
    {
        int startRow = blueprint.BaseRowMultiplier * ring;
        int startColumn = blueprint.BaseColumnMultiplier * ring;

        int rowOffset = blueprint.RowOffsetIncrement * ringSideIndex;
        int columnOffset = blueprint.ColumnOffsetIncrement * ringSideIndex;

        int finalRow = startRow + rowOffset;
        int finalColumn = startColumn + columnOffset;

        Vector2 hexPos = GetHexPos(finalRow, finalColumn, ring);
        bool playable = _main.MapDefinition.ContainsDefinitionFor(finalRow, finalColumn);
        int index = playable ? RegisterNextHex() : 0;
        return new HexCenter(hexPos, index, finalRow, finalColumn, _extents);
    }

    private Vector2 GetHexPos(int row, int ascendingColumn, int ring)
    {
        ring = ring + 2;
        Vector2 ascendingOffset = MapDisplay.AscendingTileOffset * ascendingColumn;
        Vector2 ret = ascendingOffset + new Vector2(row, 0);
        ret /= _extents * 2 - 1f;
        ret += new Vector2(.5f, .5f);
        return ret;
    }

    private int RegisterNextHex()
    {
        _currentHexIndex++;
        return _currentHexIndex;
    }

    private RingSideBlueprint[] CreateRingSideBlueprints()
    {
        RingSideBlueprint[] ret = new RingSideBlueprint[]
        {
            new RingSideBlueprint(1, 0, 0, -1),
            new RingSideBlueprint(-1, 1, 1, 0),
            new RingSideBlueprint(0, 1, 1, -1),
            new RingSideBlueprint(-1, 0, 0, 1),
            new RingSideBlueprint(1, -1, -1, 0),
            new RingSideBlueprint(0, -1, -1, 1)
        };
        return ret;
    }
    /// <summary>
    /// This hex table only exists so that base texture creation doesn't take forever at high texture-resolutions/extents.
    /// </summary>
    private class HexTable
    {
        private readonly int _extents;
        private readonly HashSet<HexCenter>[,] _tableData;

        public HexTable(IEnumerable<HexCenter> centers, int extents)
        {
            _extents = extents;
            _tableData = CreateTable(centers, extents);
        }

        private HashSet<HexCenter>[,] CreateTable(IEnumerable<HexCenter> centers, int extents)
        {
            HashSet<HexCenter>[,] ret = new HashSet<HexCenter>[extents, extents];
            for (int x = 0; x < extents; x++)
            {
                for (int y = 0; y < extents; y++)
                {
                    ret[x, y] = new HashSet<HexCenter>();
                }
            }
            foreach (HexCenter item in centers)
            {
                int tableX = (int)(item.Position.x * (extents - 1));
                int tableY = (int)(item.Position.y * (extents - 1));
                int tableXneg = Mathf.Clamp(tableX - 1, 0, extents - 1);
                int tableYneg = Mathf.Clamp(tableY - 1, 0, extents - 1);
                int tableXpos = Mathf.Clamp(tableX + 1, 0, extents - 1);
                int tableYpos = Mathf.Clamp(tableY + 1, 0, extents - 1);


                ret[tableXpos, tableY].Add(item);
                ret[tableXpos, tableYneg].Add(item);
                ret[tableXpos, tableYpos].Add(item);

                ret[tableX, tableY].Add(item);
                ret[tableX, tableYneg].Add(item);
                ret[tableX, tableYpos].Add(item);

                ret[tableXneg, tableY].Add(item);
                ret[tableXneg, tableYneg].Add(item);
                ret[tableXneg, tableYpos].Add(item);
            }
            return ret;
        }

        public HexCenter GetHexCenter(int x, int y, int imageWidth, int imageHeight)
        {
            float xParam = (float)x / imageWidth;
            float yParam = (float)y / imageHeight;
            Vector2 pixelPos = new Vector2(xParam, yParam);

            int tableX = (int)(pixelPos.x * (_extents - 1));
            int tableY = (int)(pixelPos.y * (_extents - 1));
            IEnumerable<HexCenter> cell = _tableData[tableX, tableY];
            return GetCenterFromCell(cell, pixelPos);
        }

        private HexCenter GetCenterFromCell(IEnumerable<HexCenter> cell, Vector2 pixelPos)
        {
            HexCenter ret = null;
            float minDist = float.PositiveInfinity;
            foreach (HexCenter center in cell)
            {
                float dist = (pixelPos - center.Position).magnitude;
                if (dist < minDist)
                {
                    ret = center;
                    minDist = dist;
                }
            }
            return ret;
        }
    }

    public class HexCenter
    {
        public Vector2 Position { get; }
        public int Index { get; }
        public int Row { get; }
        public int Column { get; }

        public Corners Corners { get; }
        public NeighborIndices Neighbors { get; }
        

        public HexCenter(Vector2 position, int index, int row, int column, int extents)
        {
            Position = position;
            Index = index;
            Row = row;
            Column = column;
            Corners = GetHexNeighbors(position, extents);
        }

        private Corners GetHexNeighbors(Vector2 pos, int extents)
        {
            Vector2 rowOffset = new Vector2(0, 1);
            Vector2 columnOffset = new Vector2(MapDisplay.AscendingTileOffset.y, MapDisplay.AscendingTileOffset.x);
            int scale = extents * 2 - 1;
            rowOffset /= scale;
            columnOffset /= scale;
            rowOffset /= 2;
            columnOffset /= 2;
            rowOffset *= 1.15f;
            columnOffset *= 1.15f;

            return new Corners()
            {
                CornerA = rowOffset + pos,
                CornerB = columnOffset + pos,
                CornerC = columnOffset - rowOffset + pos,
                CornerD = -rowOffset + pos,
                CornerE = -columnOffset + pos,
                CornerF = -columnOffset + rowOffset + pos,
            };
        }
    }

    class RingSideBlueprint
    {
        public int BaseRowMultiplier { get; }
        public int BaseColumnMultiplier { get; }
        public int RowOffsetIncrement { get; }
        public int ColumnOffsetIncrement { get; }

        public RingSideBlueprint(int baseRowMultiplier,
            int baseColumnMultiplier,
            int rowOffsetIncrement,
            int columnOffsetIncrement)
        {
            BaseRowMultiplier = baseRowMultiplier;
            BaseColumnMultiplier = baseColumnMultiplier;
            RowOffsetIncrement = rowOffsetIncrement;
            ColumnOffsetIncrement = columnOffsetIncrement;
        }
    }
}
