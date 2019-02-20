using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MapUnityObject : MonoBehaviour
{
    public Material MapMat;
    public Collider MapCollider;

    private ComputeBuffer _neighborsBuffer;
    private const int _neighborsBufferStride = sizeof(uint) * 6;
    
    private MapAssetSet _set;
    public Texture2D BaseMap { get { return _set.BaseMap; } }

    private const int TileStatesStride = sizeof(float) // Hover
        + sizeof(float) // Selected
        + sizeof(float) // Dragging
        + sizeof(float) // Dragged
        + sizeof(float) // Targetable
        + sizeof(float) * 3 // FactionColor
        ;

    private ComputeBuffer _tileStatesBuffer;
    private TileBufferState[] _tileBufferStates;
    
    private void Update()
    {
        MapMat.SetTexture("_MainTex", _set.BaseMap);
        MapMat.SetTexture("_BorderTex", _set.BorderMap);
        MapMat.SetBuffer("_NeighborsBuffer", _neighborsBuffer);
        MapMat.SetBuffer("_TileStates", _tileStatesBuffer);
    }

    private void OnDestroy()
    {
        _tileStatesBuffer.Dispose();
        _neighborsBuffer.Dispose();
    }

    public void Initialize(MapAssetSet assetSet)
    {
        int bufferSize = assetSet.MapSetup.Tiles.Max(item => item.BufferIndex + 1);
        _set = assetSet;
        _neighborsBuffer = CreateNeighborsBuffer(assetSet);
        _tileStatesBuffer = new ComputeBuffer(bufferSize, TileStatesStride);
        _tileBufferStates = new TileBufferState[bufferSize];
    }

    private ComputeBuffer CreateNeighborsBuffer(MapAssetSet assetSet)
    {
        ComputeBuffer ret = new ComputeBuffer(assetSet.NeighborsTable.Count, _neighborsBufferStride);
        ret.SetData(assetSet.NeighborsTable.ToArray());
        return ret;
    }

    public void UpdateTileStatesBuffer(IEnumerable<TileDisplay> tiles)
    {
        foreach (TileDisplay tile in tiles)
        {
            TileBufferState state = new TileBufferState()
            {
                Hover = tile.Hover,
                Selected = tile.Selected,
                Dragging = tile.Dragging,
                Targetable = tile.Targetable,
                FactionColor = new Vector3(tile.FactionDisplayColor.r, tile.FactionDisplayColor.g, tile.FactionDisplayColor.b)
            };
            _tileBufferStates[tile.Tile.BufferIndex] = state;
        }
        _tileStatesBuffer.SetData(_tileBufferStates);
    }

    public struct TileBufferState
    {
        public float Hover;
        public float Selected;
        public float Dragging;
        public float Dragged;
        public float Targetable;
        public Vector3 FactionColor;
    }
}
