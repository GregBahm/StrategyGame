using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DistortionMapManager
{
    private readonly MainMapManager _main;
    private readonly int _computeKernel;

    public ComputeBuffer OutputData { get; }
    private const int _outputBufferStride = sizeof(float) * 2;
    private readonly ComputeBuffer _originalPositions;

    private readonly ComputeBuffer _pixelNeighborsBuffer;
    private const int _pixelNeighborsStride = sizeof(int) * 4;
    
    struct PixelNeighbors
    {
        public int NeighborA;
        public int NeighborB;
        public int NeighborC;
        public int NeighborD;
    };

    public DistortionMapManager(MainMapManager main)
    {
        _main = main;
        _computeKernel = main.DistortCompute.FindKernel("CSMain");
        OutputData = new ComputeBuffer(_main.PixelCount, _outputBufferStride);
        _originalPositions = new ComputeBuffer(_main.PixelCount, _outputBufferStride);
        Vector2[] data = CreateOriginalPositions();
        _originalPositions.SetData(data);

        _pixelNeighborsBuffer = CreatePixelNeighborsBuffer();
    }

    int UvsToIndex(int u, int v)
    {
        return u + v * _main.BaseTexture.width;
    }

    private ComputeBuffer CreatePixelNeighborsBuffer()
    {
        ComputeBuffer ret = new ComputeBuffer(_main.PixelCount, _pixelNeighborsStride);
        PixelNeighbors[] data = new PixelNeighbors[_main.PixelCount];
        for (int x = 1; x < _main.BaseTexture.width - 1; x++)
        {
            for (int y = 1; y < _main.BaseTexture.height - 1; y++)
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

    public void Update()
    {
        _main.DistortCompute.SetBuffer(_computeKernel, "_OutputData", OutputData);
        _main.DistortCompute.SetBuffer(_computeKernel, "_OriginalPosition", _originalPositions);
        _main.DistortCompute.SetBuffer(_computeKernel, "_PixelNeighbors", _pixelNeighborsBuffer);
        _main.DistortCompute.SetFloat("_SourceImageWidth", _main.BaseTexture.width);
        _main.DistortCompute.SetFloat("_SourceImageHeight", _main.BaseTexture.height);
        _main.DistortCompute.SetFloat("_DistortionStrength", _main.DistortionStrength);
        _main.DistortCompute.SetFloat("_DistortionDrag", _main.DistortionDrag);
        _main.DistortCompute.SetFloat("_OriginPull", _main.OriginPull);
        _main.DistortCompute.SetTexture(_computeKernel, "NormalTexture", _main.NormalTexture);

        int groups = _main.PixelCount / 64;
        _main.DistortCompute.Dispatch(_computeKernel, groups, 1, 1);

        _main.DistortionMat.SetBuffer("_DistortionData", OutputData);
        _main.DistortionMat.SetFloat("_SourceImageWidth", _main.BaseTexture.width);
        _main.DistortionMat.SetFloat("_SourceImageHeight", _main.BaseTexture.height);
        _main.DistortionMat.SetTexture("_NormalTex", _main.NormalTexture);
        _main.DistortionMat.SetTexture("_MainTex", _main.BaseTexture);
        _main.DistortionMat.SetFloat("_InputOutput", _main.ShowDistortionBasis);
        _main.DistortionMat.SetFloat("_MaxIndex", _main.MaxIndex);
        _main.DistortionMat.SetBuffer("_CornersData", _main.CornerPointsBuffer);
    }

    internal Texture2D GetTexture()
    {
        RenderTexture renderTex = new RenderTexture(_main.BaseTexture.width, _main.BaseTexture.height, 0);
        Graphics.Blit(_main.BaseTexture, renderTex, _main.DistortionMat);
        Texture2D ret = new Texture2D(_main.BaseTexture.width, _main.BaseTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTex;
        ret.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        ret.Apply();
        return ret;
    }

    private Color GetPixelColor(Vector2 pixelData)
    {
        EncodedOffset xEncoding = new EncodedOffset(pixelData.x);
        EncodedOffset yEncoding = new EncodedOffset(pixelData.y);
        return new Color(xEncoding.FirstComponent, xEncoding.SecondComponent, yEncoding.FirstComponent, yEncoding.SecondComponent);
    }

    private class EncodedOffset
    {
        public float FirstComponent { get; }
        public float SecondComponent { get; }

        public EncodedOffset(float rawVal)
        {
            float offsetVal = rawVal / 2 + .5f;
            int componentA = (int)(offsetVal * byte.MaxValue * byte.MaxValue) % byte.MaxValue;
            int componentB = Mathf.FloorToInt(offsetVal * byte.MaxValue);
            FirstComponent = (float)componentA / byte.MaxValue;
            SecondComponent = (float)componentB / byte.MaxValue;
        }
    }

    public void WriteDistoredMap(string path)
    {
        Vector2[] outputData = new Vector2[_main.PixelCount];
        OutputData.GetData(outputData);
        Texture2D texture = new Texture2D(_main.BaseTexture.width, _main.BaseTexture.height);
        for (int x = 0; x < _main.BaseTexture.width; x++)
        {
            for (int y = 0; y < _main.BaseTexture.height; y++)
            {
                Vector2 datum = outputData[UvsToIndex(x, y)];
                datum = datum / 2 + new Vector2(.5f, .5f);
                texture.SetPixel(x, y, new Color(datum.x, datum.y, 0));
            }
        }
        texture.Apply();
        byte[] saveData = texture.EncodeToPNG();
        File.WriteAllBytes(path, saveData);
    }

    private Vector2[] CreateOriginalPositions()
    {
        Vector2[] data = new Vector2[_main.PixelCount];
        for (int x = 0; x < _main.BaseTexture.width; x++)
        {
            for (int y = 0; y < _main.BaseTexture.height; y++)
            {
                int index = UvsToIndex(x, y);
                float u = (float)x / _main.BaseTexture.width;
                float v = (float)y / _main.BaseTexture.height;
                data[index] = new Vector2(u, v);
            }
        }
        return data;
    }

    public void OnDestroy()
    {
        _originalPositions.Dispose();
        OutputData.Dispose();
        _pixelNeighborsBuffer.Dispose();
    }
}
