using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

    public MapDefinition MapDefinition { get; private set; }
    public Texture2D BaseTexture { get; private set; }

    public int PixelCount { get; private set; }
    public int MaxIndex { get; private set; }
    public ReadOnlyCollection<BaseMapManager.HexCenter> BaseHexs { get { return _baseMapGenerator.BaseHexs; } }

    public Material BorderMat;
    [Range(0, 0.1f)]
    public float BorderThickness;
    public bool DrawCornerDebugLines;

    private BaseMapManager _baseMapGenerator;
    private DistortionMapManager _distorter;
    private SelectionMapManager _selectionTester;
    private BorderMapManager _borderGenerator;


    public ComputeBuffer DistortionOutput { get { return _distorter.OutputData; } }
    public ComputeBuffer CornerPointsBuffer { get { return _baseMapGenerator.CornersData; } }
    public ComputeBuffer NeighborsBuffer { get { return _baseMapGenerator.NeighborsData; } }

    void Start ()
    {
        MapDefinition = new MapDefinition(MapDefinitionFile);
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

    public void SaveBaseMap(string path)
    {
        byte[] pngData = BaseTexture.EncodeToPNG();
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
