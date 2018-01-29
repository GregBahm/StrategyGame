using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldDistancesScript : MonoBehaviour 
{
    public ComputeShader Computer;

    private int _blurHorizontalKernel;
    private int _blurVerticalKernel;

    private const int SourceBufferStride = sizeof(bool) * 4; 
    private const int OutputBufferStride = sizeof(int) * 4;
    private ComputeBuffer _sourceBuffer;
    private ComputeBuffer _horizontalBuffer;
    private ComputeBuffer _outputBuffer;
    
    public const int HorizontalResolution = 256;
    public const int VerticalResolution = 128;
    public const int BattlefieldResolution = HorizontalResolution * VerticalResolution;
    private const int GroupSize = 128;
    
	void Awake ()
    {
        _blurHorizontalKernel = Computer.FindKernel("BlurHorizontal");
        _blurVerticalKernel = Computer.FindKernel("BlurVertical");
        _sourceBuffer = new ComputeBuffer(BattlefieldResolution, SourceBufferStride);
        _horizontalBuffer = new ComputeBuffer(BattlefieldResolution, OutputBufferStride);
        _outputBuffer = new ComputeBuffer(BattlefieldResolution, OutputBufferStride);
    }

    public BattlefieldDistances GetNextState(BattlefieldState state)
    {
        int horizontalGroups = HorizontalResolution / GroupSize;
        int verticalGroups = VerticalResolution / GroupSize;

        SetSourceBufferState(state);

        Computer.SetBuffer(_blurHorizontalKernel, "_SourceBuffer", _sourceBuffer);
        Computer.SetBuffer(_blurHorizontalKernel, "_HorizontalBuffer", _horizontalBuffer);
        Computer.Dispatch(_blurHorizontalKernel, horizontalGroups, 1, 1);

        Computer.SetBuffer(_blurVerticalKernel, "_HorizontalBuffer", _horizontalBuffer);
        Computer.SetBuffer(_blurVerticalKernel, "_OutputBuffer", _outputBuffer);
        Computer.Dispatch(_blurVerticalKernel, verticalGroups, 1, 1);

        return GetWeights();
    }

    public static int UvToIndex(int x, int y)
    {
        return (x * VerticalResolution) + y;
    }

    private void SetSourceBufferState(BattlefieldState state)
    {
        SourceLocationData[] data = new SourceLocationData[BattlefieldResolution];
        foreach (UnitLocation location in state.AttackerPositions)
        {
            data[UvToIndex(location.XPos, location.YPos)].AttackerPresent = true;
        }
        foreach (UnitLocation location in state.DefenderPositions)
        {
            data[UvToIndex(location.XPos, location.YPos)].DefenderPreset = true;
        }
        foreach (UnitLocation location in state.NeutralPositions)
        {
            data[UvToIndex(location.XPos, location.YPos)].NeutralPresent = true;
        }
        foreach (UnitLocation location in state.BerzerkerPositions)
        {
            data[UvToIndex(location.XPos, location.YPos)].BerzerkerPresent = true;
        }
        _sourceBuffer.SetData(data);
    }

    private BattlefieldDistances GetWeights()
    {
        BattlefieldDistance[] distances = new BattlefieldDistance[BattlefieldResolution];
        _outputBuffer.GetData(distances);
        return new BattlefieldDistances(distances);
    }

    private void OnDestroy()
    {
        _sourceBuffer.Dispose();
        _horizontalBuffer.Dispose();
        _outputBuffer.Dispose();
    }

    private struct SourceLocationData
    {
        public bool AttackerPresent;
        public bool DefenderPreset;
        public bool NeutralPresent;
        public bool BerzerkerPresent;
    }
}

public class BattlefieldDistances
{
    public readonly BattlefieldDistance[] Distances;

    public BattlefieldDistances(BattlefieldDistance[] distances)
    {
        Distances = distances;
    }
    public BattlefieldDistance GetDistanceAt(int x, int y)
    {
        int index = BattlefieldDistancesScript.UvToIndex(x, y);
        return Distances[index];
    }
}

public struct BattlefieldDistance
{
    public int AlliedDistance;
    public int EnemyDistance;
    public int NeutralDistance;
    public int BerzerkerDistance;
}

public struct BattlefieldState
{
    public UnitLocation[] AttackerPositions;
    public UnitLocation[] DefenderPositions;
    public UnitLocation[] NeutralPositions;
    public UnitLocation[] BerzerkerPositions;
}
