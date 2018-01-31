using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldMover
{
    private readonly ComputeShader _computer;

    private int _blurHorizontalKernel;
    private int _blurVerticalKernel; 

    private const int SourceBufferStride = sizeof(int) * 4; 
    private const int OutputBufferStride = sizeof(int) * 4;
    private ComputeBuffer _sourceBuffer;
    private ComputeBuffer _horizontalBuffer;
    private ComputeBuffer _outputBuffer;
    
    public const int HorizontalResolution = 256;
    public const int VerticalResolution = 128;
    public const int BattlefieldResolution = HorizontalResolution * VerticalResolution;
    private const int GroupSize = 128;

    public BattlefieldMover(ComputeShader computer)
    {
        _computer = computer;
        _blurHorizontalKernel = _computer.FindKernel("BlurHorizontal");
        _blurVerticalKernel = _computer.FindKernel("BlurVertical");
        _sourceBuffer = new ComputeBuffer(BattlefieldResolution, SourceBufferStride);
        _horizontalBuffer = new ComputeBuffer(BattlefieldResolution, OutputBufferStride);
        _outputBuffer = new ComputeBuffer(BattlefieldResolution, OutputBufferStride);
    }

    public BattlefieldDistances GetNextState(BattlefieldState state)
    {
        int horizontalGroups = HorizontalResolution / GroupSize;
        int verticalGroups = VerticalResolution / GroupSize;

        SetSourceBufferState(state);

        _computer.SetBuffer(_blurHorizontalKernel, "_SourceBuffer", _sourceBuffer);
        _computer.SetBuffer(_blurHorizontalKernel, "_HorizontalBuffer", _horizontalBuffer);
        _computer.Dispatch(_blurHorizontalKernel, horizontalGroups, 1, 1);

        _computer.SetBuffer(_blurVerticalKernel, "_HorizontalBuffer", _horizontalBuffer);
        _computer.SetBuffer(_blurVerticalKernel, "_OutputBuffer", _outputBuffer);
        _computer.Dispatch(_blurVerticalKernel, verticalGroups, 1, 1);

        return GetWeights();
    }

    public static int UvToIndex(int x, int y)
    {
        return (x * VerticalResolution) + y;
    }

    public static int UvToIndex(UnitLocation location)
    {
        return (location.XPos * VerticalResolution) + location.YPos;
    }

    private void SetSourceBufferState(BattlefieldState state)
    {
        SourceLocationData[] data = new SourceLocationData[BattlefieldResolution];
        foreach (UnitLocation location in state.AttackerPositions)
        {
            data[UvToIndex(location.XPos, location.YPos)].AttackerPresent = 1;
        }
        foreach (UnitLocation location in state.DefenderPositions)
        {
            data[UvToIndex(location.XPos, location.YPos)].DefenderPreset = 1;
        }
        foreach (UnitLocation location in state.NeutralPositions)
        {
            data[UvToIndex(location.XPos, location.YPos)].NeutralPresent = 1;
        }
        foreach (UnitLocation location in state.BerzerkerPositions)
        {
            data[UvToIndex(location.XPos, location.YPos)].BerzerkerPresent = 1;
        }
        _sourceBuffer.SetData(data);
    }

    internal BattlefieldState MoveUnits(BattlefieldState currentState, BattlefieldDistances distances)
    {
        BitArray collisionBitarray = GetCollisionBitarray(currentState);
        UnitLocation[] attackerLocations = GetMovedUnits(currentState.AttackerPositions, UnitAllegiance.Attackers, collisionBitarray, distances);
        UnitLocation[] defenderLocations = GetMovedUnits(currentState.DefenderPositions, UnitAllegiance.Defenders, collisionBitarray, distances);

        BattlefieldState state = new BattlefieldState(attackerLocations,
            defenderLocations,
            new UnitLocation[0],
            new UnitLocation[0]);
        return state;
    }

    private UnitLocation[] GetMovedUnits(UnitLocation[] positions, UnitAllegiance allegiance, BitArray collisionBitarray, BattlefieldDistances distances)
    {
        List<UnitLocation> newLocations = new List<UnitLocation>();
        foreach (UnitLocation location in positions)
        {
            UnitLocation newLocation = distances.GetNextPosition(location, allegiance, collisionBitarray, distances);
            collisionBitarray[UvToIndex(location)] = false;
            collisionBitarray[UvToIndex(newLocation)] = true;
            newLocations.Add(newLocation);
        }
        return newLocations.ToArray();
    }

    private BitArray GetCollisionBitarray(BattlefieldState currentState)
    {
        BitArray ret = new BitArray(BattlefieldResolution);
        foreach (UnitLocation location in currentState.AllUnits)
        {
            int index = UvToIndex(location);
            ret[index] = true;
        }
        return ret;
    }

    private BattlefieldDistances GetWeights()
    {
        BattlefieldDistance[] distances = new BattlefieldDistance[BattlefieldResolution];
        _outputBuffer.GetData(distances);
        return new BattlefieldDistances(distances);
    }

    public void DoDispose()
    {
        _sourceBuffer.Dispose();
        _horizontalBuffer.Dispose();
        _outputBuffer.Dispose();
    }

    private struct SourceLocationData
    {
        public int AttackerPresent;
        public int DefenderPreset;
        public int NeutralPresent;
        public int BerzerkerPresent;
    }

    public static bool IsWithinBounds(UnitLocation location)
    {
        return location.XPos >= 0
            && location.YPos >= 0
            && location.XPos < BattlefieldMover.HorizontalResolution
            && location.YPos < BattlefieldMover.VerticalResolution;
    }
}