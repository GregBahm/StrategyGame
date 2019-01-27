using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HexMapGenerator : MonoBehaviour
{
    public int Extents;
    public Material Mat;
    private Texture2D TestTexture;

    private void Start()
    {
        TestTexture = new Texture2D(1024, 1024);
        MapMap();
        SaveTexture();
    }

    private void SaveTexture()
    {
        byte[] pngData = TestTexture.EncodeToPNG();
        File.WriteAllBytes(@"C:\Users\Lisa\Documents\ArrowMaker\Assets\HexTexture.png", pngData);
    }

    private void Update()
    {
        Mat.SetTexture("_MainTex", TestTexture);
    }

    private void MapMap()
    {
        SetBaseColors();
        TestTexture.Apply();
    }

    private void SetBaseColors()
    {
        IEnumerable<HexCenter> startingPoints = GetStartingPoints().ToArray();
        for (int xIndex = 0; xIndex < TestTexture.width; xIndex++)
        {
            for (int yIndex = 0; yIndex < TestTexture.height; yIndex++)
            {
                float xParam = (float)xIndex / TestTexture.width;
                float yParam = (float)yIndex / TestTexture.height;
                Color pixelColor = GetPixelColor(new Vector2(xParam, yParam), startingPoints);
                TestTexture.SetPixel(xIndex, yIndex, pixelColor);
            }
        }
    }

    private Color GetPixelColor(Vector2 pixelPos, IEnumerable<HexCenter> startingPoints)
    {
        int hexIndex = 0;
        float minDist = float.PositiveInfinity;
        foreach (HexCenter center in startingPoints)
        {
            float dist = (pixelPos - center.Position).magnitude;
            if(dist < minDist)
            {
                hexIndex = center.Index;
                minDist = dist;
            }
        }

        float param = (float)hexIndex / 255;
        Color ret = new Color(param, 0, 0);
        return ret;
    }

    private IEnumerable<HexCenter> GetStartingPoints()
    {
        RingSideBlueprint[] ringSideBlueprints = CreateRingSideBlueprints();
        yield return new HexCenter(new Vector2(.5f, .5f), GetNewHexIndex());
        for (int ring = 1; ring < Extents; ring++)
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
        return new HexCenter(hexPos, GetNewHexIndex());
    }

    private Vector2 GetHexPos(int row, int ascendingColumn, int ring)
    {
        ring = ring + 2;
        float y = -Mathf.RoundToInt(ring / 5);
        Vector2 ascendingOffset = MapDisplay.AscendingTileOffset * ascendingColumn;
        Vector2 offset = ascendingOffset + new Vector2(row, 0);
        offset *= 2;
        Vector2 ret = new Vector2(offset.x, offset.y);
        ret /= Extents * 2;
        ret += new Vector2(.5f, .5f);
        return ret;
    }

    private int _hexIndex;

    private int GetNewHexIndex()
    {
        _hexIndex++;
        return _hexIndex;
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
