using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BaseMapGenerator))]
public class SelectionTester : MonoBehaviour
{
    public Material DisplayMat;
    [Range(0, 1)]
    public float HoverShiftSpeed;

    private int _hexCount;
    private const int HexStatesStride = sizeof(float) * 2;
    private ComputeBuffer _hexStatesBuffer;
    private Texture2D _texture;

    private HexState[] _hexStates;

    private void Start()
    {
        BaseMapGenerator mapGen = GetComponent<BaseMapGenerator>();
        _hexCount = mapGen.MapDefinition.Tiles.Count();
        _texture = mapGen.OutputTexture;

        _hexStatesBuffer = new ComputeBuffer(_hexCount, HexStatesStride);
        _hexStates = CreateHexStates();
    }

    void Update()
    {
        HexState hexState = GetHoveredState();
        UpdateHexStates(hexState);
        SetBufferData();

        DisplayMat.SetTexture("_MainTex", _texture);
        DisplayMat.SetBuffer("_HexStates", _hexStatesBuffer);
    }

    private void OnDestroy()
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
            state.Hover = Mathf.Lerp(target, state.Hover, HoverShiftSpeed);
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

    private HexState GetHoveredState()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity);
        if (hit)
        {
            Vector2 coord = hitInfo.textureCoord;
            int x = (int)(_texture.width * coord.x);
            int y = (int)(_texture.height * coord.y);
            Color col = _texture.GetPixel(x, y);
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
