using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Distorter : MonoBehaviour
{
    public bool WriteDistoredMap;

    private int _computeKernel;
    [Range(0, .01f)]
    public float DistortionStrength;
    [Range(0, 1)]
    public float DistortionDrag;
    [Range(0, 1)]
    public float OriginPull;
    [Range(0, 1)]
    public float InputOutput;
    public ComputeShader DistortCompute;
    public Texture2D HexTexture;
    public Texture2D NormalTexture;

    public Material OutputMat;

    private ComputeBuffer _outputBuffer;
    private const int _outputBufferStride = sizeof(float) * 2;
    private ComputeBuffer _originalPositions;

    private ComputeBuffer _pixelNeighborsBuffer;
    private const int _pixelNeighborsStride = sizeof(int) * 4;

    private int _pixelCount;
    
    struct PixelNeighbors
    {
        public int NeighborA;
        public int NeighborB;
        public int NeighborC;
        public int NeighborD;
    };

    private void Start()
    {
        _computeKernel = DistortCompute.FindKernel("CSMain");
        _pixelCount = HexTexture.width * HexTexture.height;
        _outputBuffer = new ComputeBuffer(_pixelCount, _outputBufferStride);
        _originalPositions = new ComputeBuffer(_pixelCount, _outputBufferStride);
        Vector2[] data = CreateOriginalPositions();
        _outputBuffer.SetData(data);
        _originalPositions.SetData(data);

        _pixelNeighborsBuffer = CreatePixelNeighborsBuffer();
    }

    int UvsToIndex(int u, int v)
    {
        return u + v * HexTexture.width;
    }

    private ComputeBuffer CreatePixelNeighborsBuffer()
    {
        ComputeBuffer ret = new ComputeBuffer(_pixelCount, _pixelNeighborsStride);
        PixelNeighbors[] data = new PixelNeighbors[_pixelCount];
        for (int x = 1; x < HexTexture.width - 1; x++)
        {
            for (int y = 1; y < HexTexture.height - 1; y++)
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
        DistortCompute.SetBuffer(_computeKernel, "_OutputData", _outputBuffer);
        DistortCompute.SetBuffer(_computeKernel, "_OriginalPosition", _originalPositions);
        DistortCompute.SetBuffer(_computeKernel, "_PixelNeighbors", _pixelNeighborsBuffer);
        DistortCompute.SetFloat("_SourceImageWidth", HexTexture.width);
        DistortCompute.SetFloat("_SourceImageHeight", HexTexture.height);
        DistortCompute.SetFloat("_DistortionStrength", DistortionStrength);
        DistortCompute.SetFloat("_DistortionDrag", DistortionDrag);
        DistortCompute.SetFloat("_OriginPull", OriginPull);
        DistortCompute.SetTexture(_computeKernel, "NormalTexture", NormalTexture);

        int groups = _pixelCount / 64;
        DistortCompute.Dispatch(_computeKernel, groups, 1, 1);

        OutputMat.SetBuffer("_OutputData", _outputBuffer);
        OutputMat.SetFloat("_SourceImageWidth", HexTexture.width);
        OutputMat.SetFloat("_SourceImageHeight", HexTexture.height);
        OutputMat.SetTexture("_NormalTex", NormalTexture);
        OutputMat.SetTexture("_HexTex", HexTexture);
        OutputMat.SetFloat("_InputOutput", InputOutput);


        if (WriteDistoredMap)
        {
            DoWriteDistoredMap();
            WriteDistoredMap = false;
        }
    }

    private void DoWriteDistoredMap()
    {
        Vector2[] outputData = new Vector2[_pixelCount];
        _outputBuffer.GetData(outputData);
        Texture2D texture = new Texture2D(HexTexture.width, HexTexture.height);
        for (int x = 0; x < HexTexture.width; x++)
        {
            for (int y = 0; y < HexTexture.height; y++)
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
        int index = 0;
        for (int x = 0; x < HexTexture.width; x++)
        {
            for (int y = 0; y < HexTexture.height; y++)
            {
                float u = (float)x / HexTexture.width;
                float v = (float)y / HexTexture.height;
                data[index] = new Vector2(u, v);
                index++;
            }
        }
        return data;
    }

    private void OnDestroy()
    {
        _originalPositions.Dispose();
        _outputBuffer.Dispose();
        _pixelNeighborsBuffer.Dispose();
    }
}
