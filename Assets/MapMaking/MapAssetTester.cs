using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAssetTester : MonoBehaviour
{
    public MapAssetSetup MapSetup;
    public Material MapTesterMat;
    private MapAssetSet _set;
    private ComputeBuffer _neighborsBuffer;
    private const int _neighborsBufferStride = sizeof(uint) * 6;

    private void Start()
    {
        _set = MapSetup.GetMapAssetSet();
        _neighborsBuffer = new ComputeBuffer(_set.NeighborsTable.Count, _neighborsBufferStride);
    }

    private void Update()
    {
        MapTesterMat.SetTexture("_MainTex", _set.BaseMap);
        MapTesterMat.SetTexture("_BorderTex", _set.BorderMap);
        MapTesterMat.SetBuffer("_NeighborsBuffer", _neighborsBuffer);
    }
}
