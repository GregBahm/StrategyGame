using System;
using System.Collections.Generic;
using UnityEngine;
public class Battlefield
{
    private readonly int _width;
    public int Width { get{ return _width; } }

    private readonly int _height;
    public int Height { get{ return _height; } }

    private readonly ComputeShader _computer;
    private readonly ComputeBuffer _unitPositionsBuffer;
    private readonly ComputeBuffer _mapWeightsBuffer;
    private readonly int _mapWeightKernel;
    private readonly int _mapClearKernel;

    private const int GroupSize = 128;
    private readonly int _mapClearGroups;

    public Battlefield(int width, int height, ComputeShader computer)
    {
        _width = width;
        _height = height;
        _computer = computer;
        _mapWeightKernel = computer.FindKernel("WeightTheMap");
        _mapClearKernel = computer.FindKernel("ClearTheMap");

    }

    public void Release()
    {
        _unitPositionsBuffer.Release();
        _mapWeightsBuffer.Release();
    }

    internal void UpdatePositions(List<UnitState> units)
    {
        // TODO: Position Updating System
        throw new NotImplementedException();
    }

    public UnitState GetUnitAt(int x, int y)
    {
        throw new NotImplementedException();
    }

    public UnitState GetUnitAt(UnitPosition pos)
    {
        return GetUnitAt(pos.XPos, pos.YPos);
    }

    public UnitState GetUnitAt(UnitLocation pos)
    {
        return GetUnitAt(pos.XPos, pos.YPos);
    }
    
    internal IEnumerable<UnitState> GetRangedTargetFor(UnitState unit, RangedAttack rangedAttack)
    {
        throw new NotImplementedException();
    }
}