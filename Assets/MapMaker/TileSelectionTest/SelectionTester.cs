using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionTester : MonoBehaviour
{
    public struct HexBufferState
    {
        public float Hover;
        public float Clicked;
    }

    private const int HexCount = 37;
    private const int HexStatesStride = sizeof(float) * 2;
    private ComputeBuffer HexStatesBuffer;
    public Texture2D Texture;
    public Material DisplayMat;
    [Range(0, 1)]
    public float HoverShiftSpeed;

    public HexState[] HexStates;

    public class HexState
    {
        public float Hover { get; set; }
        public bool Clicked { get; set; }
    }

    private void Start()
    {
        HexStatesBuffer = new ComputeBuffer(HexCount, HexStatesStride);
        HexStates = CreateHexStates();
    }

    private HexState[] CreateHexStates()
    {
        HexState[] ret = new HexState[HexCount];
        for (int i = 0; i < HexCount; i++)
        {
            ret[i] = new HexState();
        }
        return ret;
    }

    void Update ()
    {
        HexState hexState = GetHoveredState();
        UpdateHexStates(hexState);
        SetBufferData();

        DisplayMat.SetBuffer("_HexStates", HexStatesBuffer);
    }

    private void SetBufferData()
    {
        HexBufferState[] data = new HexBufferState[HexCount];
        for (int i = 0; i < HexCount; i++)
        {
            HexState source = HexStates[i];
            data[i] = new HexBufferState() { Hover = source.Hover, Clicked = source.Clicked ? 1 : 0};
        }
        HexStatesBuffer.SetData(data);
    }

    private void UpdateHexStates(HexState hoveredState)
    {
        bool clicked = Input.GetMouseButtonDown(0);
        foreach (HexState state in HexStates)
        {
            float target = state == hoveredState ? 1f : 0;
            state.Hover = Mathf.Lerp(target, state.Hover, HoverShiftSpeed);
            if(clicked && state == hoveredState)
            {
                state.Clicked = !state.Clicked;
            }
        }
    }

    private HexState GetHoveredState()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity);
        if (hit)
        {
            Vector2 coord = hitInfo.textureCoord;
            int x = (int)(Texture.width * coord.x);
            int y = (int)(Texture.height * coord.y);
            Color col = Texture.GetPixel(x, y);
            int index = (int)(col.r * 255);
            if(index <= HexCount - 1)
            {
                return HexStates[index];
            }
        }
        return null;
    }
}
