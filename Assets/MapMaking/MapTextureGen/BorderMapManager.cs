using System;
using UnityEngine;

internal class BorderMapManager
{
    private readonly MainMapManager _main;

    public BorderMapManager(MainMapManager main)
    {
        _main = main;
    }

    public void Update()
    {
        _main.BorderMat.SetTexture("_MainTex", _main.BaseTexture);
        _main.BorderMat.SetBuffer("_DistortionData", _main.DistortionOutput);
        _main.BorderMat.SetFloat("_SourceImageWidth", _main.BaseTexture.width);
        _main.BorderMat.SetFloat("_SourceImageHeight", _main.BaseTexture.height);
        _main.BorderMat.SetFloat("_BorderThickness", _main.BorderThickness);
        _main.BorderMat.SetBuffer("_CornersData", _main.CornerPointsBuffer);
        _main.BorderMat.SetBuffer("_NeighborsBuffer", _main.NeighborsBuffer);
    }

    internal Texture2D GetTexture()
    {
        RenderTexture renderTex = new RenderTexture(_main.BaseTexture.width, _main.BaseTexture.height, 0);
        Graphics.Blit(_main.BaseTexture, renderTex, _main.BorderMat);
        Texture2D ret = new Texture2D(_main.BaseTexture.width, _main.BaseTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTex;
        ret.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        ret.Apply();
        return ret;
    }
}