using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderGenerator : MonoBehaviour
{
    private int _computeKernel;
    public ComputeShader BorderGenCompute;
    public Texture2D SourceTexture;
    public float BorderThickness;
    public float Brightness;

    private ComputeBuffer _outputBuffer;
    private const int _outputBufferStride = sizeof(float);

    private int _pixelCount;

    public Material OutputMat;

    private void Start()
    {
        _computeKernel = BorderGenCompute.FindKernel("CSMain");
        _pixelCount = SourceTexture.width * SourceTexture.height;
        _outputBuffer = new ComputeBuffer(_pixelCount, _outputBufferStride);
    }

    private void Update()
    {
        BorderGenCompute.SetFloat("_BorderThickness", BorderThickness);
        BorderGenCompute.SetBuffer(_computeKernel, "_OutputData", _outputBuffer);
        BorderGenCompute.SetFloat("_SourceImageWidth", SourceTexture.width);
        BorderGenCompute.SetFloat("_SourceImageHeight", SourceTexture.height);
        BorderGenCompute.SetTexture(_computeKernel, "SourceImage", SourceTexture);

        int groups = _pixelCount / 64;
        BorderGenCompute.Dispatch(_computeKernel, groups, 1, 1);

        OutputMat.SetBuffer("_OutputData", _outputBuffer);
        OutputMat.SetFloat("_SourceImageWidth", SourceTexture.width);
        OutputMat.SetFloat("_SourceImageHeight", SourceTexture.height);
        OutputMat.SetFloat("_Brightness", Brightness);
    }

    private void OnDestroy()
    {
        _outputBuffer.Dispose();
    }
}
