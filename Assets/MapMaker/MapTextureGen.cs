using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapTextureGen : MonoBehaviour
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
    public ComputeBuffer DistortionOutput { get { return _distorter.OutputData; } }

    private BaseMapGenerator _baseMapGenerator;
    private Distorter _distorter;
    private SelectionTester _selectionTester;

    void Start ()
    {
        MapDefinition = new MapDefinition(MapDefinitionFile);
        BaseTexture = InitializeMap();
        PixelCount = BaseTexture.width * BaseTexture.height;
        _baseMapGenerator = new BaseMapGenerator(this);
        MaxIndex = _baseMapGenerator.MaxIndex;
        _distorter = new Distorter(this);
        _selectionTester = new SelectionTester(this);
    }

    private void Update()
    {
        _baseMapGenerator.Update();
        _distorter.Update();
        _selectionTester.Update();
    }

    private void OnDestroy()
    {
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
