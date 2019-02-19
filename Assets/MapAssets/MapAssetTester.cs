using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapAssetTester : MonoBehaviour
{
    public MapAssetSetup MapSetup;
    public Material MapTesterMat;
    [Range(0, 1)]
    public float HoverSpeed;
    private MapAssetSet _set;
    private ComputeBuffer _neighborsBuffer;
    private const int _neighborsBufferStride = sizeof(uint) * 6;


    private int _hexCount;
    private const int HexStatesStride = sizeof(float) * 2;
    private ComputeBuffer _hexStatesBuffer;

    private HexState[] _hexStates;

    private void Start()
    {
        _set = MapSetup.GetMapAssetSet();
        _hexCount = _set.MapSetup.Tiles.Count;
        _neighborsBuffer = CreateNeighborsBuffer();
        _hexStatesBuffer = new ComputeBuffer(_hexCount, HexStatesStride);
        _hexStates = CreateHexStates();
    }

    private ComputeBuffer CreateNeighborsBuffer()
    {
        ComputeBuffer ret = new ComputeBuffer(_set.NeighborsTable.Count, _neighborsBufferStride);
        ret.SetData(_set.NeighborsTable.ToArray());
        return ret;
    }

    private void OnDestroy()
    {
        _hexStatesBuffer.Dispose();
        _neighborsBuffer.Dispose();
    }

    private void Update()
    {
        HexState hexState = GetHoveredState();
        UpdateHexStates(hexState);
        SetBufferData();

        MapTesterMat.SetTexture("_MainTex", _set.BaseMap);
        MapTesterMat.SetTexture("_BorderTex", _set.BorderMap);
        MapTesterMat.SetBuffer("_NeighborsBuffer", _neighborsBuffer);
        MapTesterMat.SetBuffer("_HexStates", _hexStatesBuffer);
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


    private void UpdateHexStates(HexState hoveredState)
    {
        bool clicked = Input.GetMouseButtonDown(0);
        foreach (HexState state in _hexStates)
        {
            float target = state == hoveredState ? 1f : 0;
            state.Hover = Mathf.Lerp(target, state.Hover, HoverSpeed);
            if (clicked && state == hoveredState)
            {
                state.Clicked = !state.Clicked;
            }
        }
    }

    private void SetBufferData()
    {
        HexBufferState[] data = new HexBufferState[_hexCount];
        for (int i = 0; i < _hexCount; i++)
        {
            HexState source = _hexStates[i];
            data[i] = new HexBufferState() { Hover = source.Hover, Clicked = source.Clicked ? 1 : 0 };
        }
        _hexStatesBuffer.SetData(data);
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

    private Color GetTextureSample(Vector2 textureCoord)
    {
        int xPixel = (int)(textureCoord.x * _set.BaseMap.width);
        int yPixel = (int)(textureCoord.y * _set.BaseMap.height);
        return _set.BaseMap.GetPixel(xPixel, yPixel);
    }

    private static int HexColorToIndex(Color col)
    {
        int x = (int)(col.r * 255);
        int y = (int)(col.g * 255);
        return x + y * 255;
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
