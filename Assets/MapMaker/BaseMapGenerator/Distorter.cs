using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[RequireComponent(typeof(BaseMapGenerator))]
public class Distorter : MonoBehaviour
{
    private int _computeKernel;
    [Range(0, .01f)]
    public float DistortionStrength;
    [Range(0, 1)]
    public float DistortionDrag;
    [Range(0, 1)]
    public float OriginPull;
    [Range(0, 1)]
    public float InputOutput;
    public Texture2D NormalTexture;
    public ComputeShader DistortCompute;

    public Material OutputMat;

    public ComputeBuffer OutputData;
    private const int _outputBufferStride = sizeof(float) * 2;
    private ComputeBuffer _originalPositions;

    private ComputeBuffer _pixelNeighborsBuffer;
    private const int _pixelNeighborsStride = sizeof(int) * 4;

    private Texture2D _displayTexture;
    private int _pixelCount;
    private int _maxIndex;
    
    struct PixelNeighbors
    {
        public int NeighborA;
        public int NeighborB;
        public int NeighborC;
        public int NeighborD;
    };

    private void Start()
    {
        BaseMapGenerator baseMapGen = GetComponent<BaseMapGenerator>();
        _displayTexture = baseMapGen.OutputTexture;
        _maxIndex = baseMapGen.MaxIndex;
        _computeKernel = DistortCompute.FindKernel("CSMain");
        _pixelCount = _displayTexture.width * _displayTexture.height;
        OutputData = new ComputeBuffer(_pixelCount, _outputBufferStride);
        _originalPositions = new ComputeBuffer(_pixelCount, _outputBufferStride);
        Vector2[] data = CreateOriginalPositions();
        OutputData.SetData(data);
        _originalPositions.SetData(data);

        _pixelNeighborsBuffer = CreatePixelNeighborsBuffer();
    }

    int UvsToIndex(int u, int v)
    {
        return u + v * _displayTexture.width;
    }

    private ComputeBuffer CreatePixelNeighborsBuffer()
    {
        ComputeBuffer ret = new ComputeBuffer(_pixelCount, _pixelNeighborsStride);
        PixelNeighbors[] data = new PixelNeighbors[_pixelCount];
        for (int x = 1; x < _displayTexture.width - 1; x++)
        {
            for (int y = 1; y < _displayTexture.height - 1; y++)
            {
                int neighborA = UvsToIndex(x - 1, y);
                int neighborB = UvsToIndex(x + 1, y);
                int neighborC = UvsToIndex(x, y - 1);
                int neighborD = UvsToIndex(x, y + 1);
                int index = UvsToIndex(x, y);
                PixelNeighbors retItem = new PixelNeighbors()
                {
                    NeighborA = neighborA,
                    NeighborB = neighborB,
                    NeighborC = neighborC,
                    NeighborD = neighborD
                };
                data[index] = retItem;
            }
        }
        ret.SetData(data);
        return ret;
    }

    private void Update()
    {
        DistortCompute.SetBuffer(_computeKernel, "_OutputData", OutputData);
        DistortCompute.SetBuffer(_computeKernel, "_OriginalPosition", _originalPositions);
        DistortCompute.SetBuffer(_computeKernel, "_PixelNeighbors", _pixelNeighborsBuffer);
        DistortCompute.SetFloat("_SourceImageWidth", _displayTexture.width);
        DistortCompute.SetFloat("_SourceImageHeight", _displayTexture.height);
        DistortCompute.SetFloat("_DistortionStrength", DistortionStrength);
        DistortCompute.SetFloat("_DistortionDrag", DistortionDrag);
        DistortCompute.SetFloat("_OriginPull", OriginPull);
        DistortCompute.SetTexture(_computeKernel, "NormalTexture", NormalTexture);

        int groups = _pixelCount / 64;
        DistortCompute.Dispatch(_computeKernel, groups, 1, 1);

        OutputMat.SetBuffer("_DistortionData", OutputData);
        OutputMat.SetFloat("_SourceImageWidth", _displayTexture.width);
        OutputMat.SetFloat("_SourceImageHeight", _displayTexture.height);
        OutputMat.SetTexture("_NormalTex", NormalTexture);
        OutputMat.SetTexture("_MainTex", _displayTexture);
        OutputMat.SetFloat("_InputOutput", InputOutput);
        OutputMat.SetFloat("_MaxIndex", _maxIndex);
    }

    private void DoWriteDistoredMap()
    {
        Vector2[] outputData = new Vector2[_pixelCount];
        OutputData.GetData(outputData);
        Texture2D texture = new Texture2D(_displayTexture.width, _displayTexture.height);
        for (int x = 0; x < _displayTexture.width; x++)
        {
            for (int y = 0; y < _displayTexture.height; y++)
            {
                Vector2 datum = outputData[UvsToIndex(x, y)];
                datum = datum / 2 + new Vector2(.5f, .5f);
                texture.SetPixel(x, y, new Color(datum.x, datum.y, 0));
            }
        }
        texture.Apply();
        byte[] saveData = texture.EncodeToPNG();
        File.WriteAllBytes(@"C:\Users\Lisa\Documents\ArrowMaker\Assets\DistortionTest\DistortedMap.png", saveData);
    }

    private Vector2[] CreateOriginalPositions()
    {
        Vector2[] data = new Vector2[_pixelCount];
        for (int x = 0; x < _displayTexture.width; x++)
        {
            for (int y = 0; y < _displayTexture.height; y++)
            {
                int index = UvsToIndex(x, y);
                float u = (float)x / _displayTexture.width;
                float v = (float)y / _displayTexture.height;
                data[index] = new Vector2(u, v);
            }
        }
        return data;
    }

    private void OnDestroy()
    {
        _originalPositions.Dispose();
        OutputData.Dispose();
        _pixelNeighborsBuffer.Dispose();
    }
}
