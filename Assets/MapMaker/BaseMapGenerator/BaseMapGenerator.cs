using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BaseMapGenerator : MonoBehaviour
{
    public Material OutputDisplayMat;
    public MapDefinition MapDefinition;
    public TextAsset MapDefinitionFile;
    public MapResolution Resolution;

    public Texture2D OutputTexture { get; private set; }
    private int _maxIndex = 1;
    private int _extents;
    private int _currentHexIndex;

    private void Awake()
    {
        MapDefinition = new MapDefinition(MapDefinitionFile);
        _extents = MapDefinition.Tiles.Max(item => item.Row) + 2; // The "+ 2" is to add a dummy ring around the playable tiles
        OutputTexture = InitializeMap();
        HexCenter[] hexCenterPoints = GetHexCenterPoints().ToArray();
        _maxIndex = hexCenterPoints.Length;
        MakeMap(hexCenterPoints);
        //SaveTexture();
    }

    private Texture2D InitializeMap()
    {
        int power = (int)Resolution;
        int resolution = 1024 * power;
        Texture2D ret = new Texture2D(resolution, resolution);
        ret.filterMode = FilterMode.Point;
        return ret;
    }

    private void SaveTexture()
    {
        byte[] pngData = OutputTexture.EncodeToPNG();
        File.WriteAllBytes(@"C:\Users\Lisa\Documents\ArrowMaker\Assets\HexTexture.png", pngData);
    }

    private void Update()
    {
        OutputDisplayMat.SetTexture("_MainTex", OutputTexture);
        OutputDisplayMat.SetFloat("_MaxIndex", _maxIndex);
    }

    private void MakeMap(IEnumerable<HexCenter> hexCenters)
    {
        HexTable table = new HexTable(hexCenters, _extents);
        for (int xIndex = 0; xIndex < OutputTexture.width; xIndex++)
        {
            for (int yIndex = 0; yIndex < OutputTexture.height; yIndex++)
            {
                HexCenter hexCenter = table.GetHexCenter(xIndex, yIndex, OutputTexture.width, OutputTexture.height);
                Color pixelColor = GetMapValue(hexCenter);
                OutputTexture.SetPixel(xIndex, yIndex, pixelColor);
            }
        }
        OutputTexture.Apply();
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
        yield return new HexCenter(new Vector2(.5f, .5f), RegisterNextHex()); // The hex in the center of the map
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
        bool playable = MapDefinition.ContainsDefinitionFor(finalRow, finalColumn);
        int index = playable ? RegisterNextHex() : 0;
        return new HexCenter(hexPos, index);
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

    public enum MapResolution
    {
        OneK = 1,
        TwoK = 2,
        FourK = 4,
        EightK = 8
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

    class HexCenter
    {
        public Vector2 Position { get; }
        public int Index { get; }
        public HexCenter(Vector2 position, int index)
        {
            Position = position;
            Index = index;
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
