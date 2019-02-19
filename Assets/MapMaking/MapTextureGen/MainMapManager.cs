using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class MainMapManager : MonoBehaviour
{
    public Material BaseMapMat;
    public TextAsset MapDefinitionFile;
    public MapResolution Resolution;

    [Range(0, .01f)]
    public float DistortionStrength;
    [Range(0, 1)]
    public float DistortionDrag;
    [Range(0, 1)]
    public float OriginPull;
    [Range(0, 1)]
    public float ShowDistortionBasis;
    public Texture2D NormalTexture;
    public ComputeShader DistortCompute;
    public Material DistortionMat;

    public Material SelectionTestMat;
    [Range(0, 1)]
    public float SelectionTestHoverSpeed;

    public MapTilesBasis MapDefinition { get; private set; }
    public Texture2D BaseTexture { get; private set; }

    public int PixelCount { get; private set; }
    public int MaxIndex { get; private set; }
    public ReadOnlyCollection<BaseMapManager.HexCenter> BaseHexs { get { return _baseMapGenerator.BaseHexs; } }

    public Material BorderMat;
    [Range(0, 0.1f)]
    public float BorderThickness;
    public bool DrawCornerDebugLines;

    public bool ExportMaps;

    private BaseMapManager _baseMapGenerator;
    private DistortionMapManager _distorter;
    private SelectionMapManager _selectionTester;
    private BorderMapManager _borderGenerator;


    public ComputeBuffer DistortionOutput { get { return _distorter.OutputData; } }
    public ComputeBuffer CornerPointsBuffer { get { return _baseMapGenerator.CornersData; } }
    public ComputeBuffer NeighborsBuffer { get { return _baseMapGenerator.NeighborsData; } }

    void Start ()
    {
        MapDefinition = new MapTilesBasis(MapDefinitionFile);
        BaseTexture = InitializeMap();
        PixelCount = BaseTexture.width * BaseTexture.height;
        _baseMapGenerator = new BaseMapManager(this);
        MaxIndex = _baseMapGenerator.MaxIndex;
        _distorter = new DistortionMapManager(this);
        _selectionTester = new SelectionMapManager(this);
        _borderGenerator = new BorderMapManager(this);
    }

    private void Update()
    {
        _baseMapGenerator.Update();
        _distorter.Update();
        _selectionTester.Update();
        _borderGenerator.Update();

        if(ExportMaps)
        {
            ExportMaps = false;
            DoExportMaps();
        }
    }

    private void OnDestroy()
    {
        _baseMapGenerator.OnDestroy();
        _distorter.OnDestroy();
        _selectionTester.OnDestroy();
    }

    private Texture2D InitializeMap()
    {
        int power = (int)Resolution;
        int resolution = 1024 * power;
        Texture2D ret = new Texture2D(resolution, resolution, TextureFormat.RG16, false);
        ret.filterMode = FilterMode.Point;
        return ret;
    }

    private void DoExportMaps()
    {
        string baseDirectory = Application.dataPath + "\\MapAssets\\";

        Texture2D distortedBaseMap = _distorter.GetTexture();
        string baseMapPath = baseDirectory + "MapBase.png";
        SaveMap(baseMapPath, distortedBaseMap);

        Texture2D bordersMap = _borderGenerator.GetTexture();
        string bordersMapPath = baseDirectory + "MapBorders.png";
        SaveMap(bordersMapPath, bordersMap);
        
        string tileSetupPath = baseDirectory + "MapTileSetup.txt";
        string tileSetupText = GetTileSetupText(distortedBaseMap);
        File.WriteAllText(tileSetupPath, tileSetupText);
    }

    private string GetTileSetupText(Texture2D distortedBaseMap)
    {
        StringBuilder ret = new StringBuilder();
        Vector2[] data = GetTileCenters(distortedBaseMap).ToArray();
        foreach (BaseMapManager.HexCenter hex in BaseHexs)
        {
            Vector2 vect = data[hex.Index];

            ret.AppendFormat("{0} {1} {2} {3} {4}\n",hex.Row, hex.Column, hex.Index, vect.x, vect.y);
        }
        return ret.ToString().Trim();
    }

    private IEnumerable<Vector2> GetTileCenters(Texture2D distortedBaseMap)
    {
        TileCenterCounter[] centerCounters = new TileCenterCounter[MaxIndex];
        for (int i = 0; i < MaxIndex; i++)
        {
            centerCounters[i] = new TileCenterCounter();
        }

        for (int x = 0; x < BaseTexture.width; x++)
        {
            for (int y = 0; y < BaseTexture.height; y++)
            {
                Color pixel = BaseTexture.GetPixel(x, y);
                int index = ToIndex(pixel);
                float xParam = (float)x / BaseTexture.width;
                float yParam = (float)y / BaseTexture.height;
                centerCounters[index].RegisterPixel(xParam, yParam);
            }
        }
        return centerCounters.Select(item => item.GetTileCenter());
    }

    private int ToIndex(Color pixel)
    {
        int x = (int)(pixel.r * byte.MaxValue);
        int y = (int)(pixel.g * byte.MaxValue) * byte.MaxValue;
        return x + y;
    }

    private class TileCenterCounter
    {
        private int _pixelCount;
        private float _sumX;
        private float _sumY;
        public void RegisterPixel(float x, float y)
        {
            _pixelCount++;
            _sumX += x;
            _sumY += y;
        }

        public Vector2 GetTileCenter()
        {
            float retX = _sumX / _pixelCount;
            float retY = _sumY / _pixelCount;
            return new Vector2(retX, retY);
        }
    }

    private static void SaveMap(string path, Texture2D texture)
    {
        byte[] pngData = texture.EncodeToPNG();
        File.WriteAllBytes(path, pngData);
    }

    public enum MapResolution
    {
        OneK = 1,
        TwoK = 2,
        FourK = 4,
        EightK = 8
    }

}
