using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionTester
{
    private readonly MapTextureGen _main;

    private int _hexCount;
    private const int HexStatesStride = sizeof(float) * 2;
    private ComputeBuffer _hexStatesBuffer;

    private HexState[] _hexStates;

    public SelectionTester(MapTextureGen main)
    {
        _main = main;
        _hexCount = main.MapDefinition.Tiles.Count();

        _hexStatesBuffer = new ComputeBuffer(_hexCount, HexStatesStride);
        _hexStates = CreateHexStates();
    }

    public void Update()
    {
        HexState hexState = GetHoveredState();
        UpdateHexStates(hexState);
        SetBufferData();

        _main.SelectionTestMat.SetBuffer("_DistortionData", _main.DistortionOutput);
        _main.SelectionTestMat.SetFloat("_SourceImageWidth", _main.BaseTexture.width);
        _main.SelectionTestMat.SetFloat("_SourceImageHeight", _main.BaseTexture.height);
        _main.SelectionTestMat.SetTexture("_MainTex", _main.BaseTexture);
        _main.SelectionTestMat.SetBuffer("_HexStates", _hexStatesBuffer);
    }

    public void OnDestroy()
    {
        _hexStatesBuffer.Dispose();
    }

    private HexState[] CreateHexStates()
    {
        HexState[] ret = new HexState[_hexCount];
        for (int i = 0; i < _hexCount; i++)
        {
            ret[i] = new HexState();
        }
        return ret;
    }

    private void SetBufferData()
    {
        HexBufferState[] data = new HexBufferState[_hexCount];
        for (int i = 0; i < _hexCount; i++)
        {
            HexState source = _hexStates[i];
            data[i] = new HexBufferState() { Hover = source.Hover, Clicked = source.Clicked ? 1 : 0};
        }
        _hexStatesBuffer.SetData(data);
    }

    private void UpdateHexStates(HexState hoveredState)
    {
        bool clicked = Input.GetMouseButtonDown(0);
        foreach (HexState state in _hexStates)
        {
            float target = state == hoveredState ? 1f : 0;
            state.Hover = Mathf.Lerp(target, state.Hover, _main.SelectionTestHoverSpeed);
            if(clicked && state == hoveredState)
            {
                state.Clicked = !state.Clicked;
            }
        }
    }

    private static int HexColorToIndex(Color col)
    {
        int x = (int)(col.r * 255);
        int y = (int)(col.g * 255);
        return x + y * 255;
    }

    private int GetDistortionIndex(int x, int y)
    {
        return x + y * _main.BaseTexture.width;
    }

    Color GetTextureSample(Vector2 coord)
    {
        int x = (int)(_main.BaseTexture.width * coord.x);
        int y = (int)(_main.BaseTexture.height * coord.y);

        int distortionIndex = GetDistortionIndex(x, y);
        float[] datum = new float[2];
        _main.DistortionOutput.GetData(datum, 0, distortionIndex, 2);

        int distortedX = (int)(_main.BaseTexture.width * datum[0]);
        int distortedY = (int)(_main.BaseTexture.height * datum[1]);

        return _main.BaseTexture.GetPixel(distortedX, distortedY);
    }

    private HexState GetHoveredState()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity);
        if (hit)
        {
            Color col = GetTextureSample(hitInfo.textureCoord);
            int index = HexColorToIndex(col);
            if (index <= _hexCount - 1)
            {
                return _hexStates[index];
            }
        }
        return null;
    }

    private struct HexBufferState
    {
        public float Hover;
        public float Clicked;
    }

    private class HexState
    {
        public float Hover { get; set; }
        public bool Clicked { get; set; }
    }
}
