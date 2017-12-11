using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleRound
{
    private readonly IEnumerable<UnitStateRecord> _attackingUnits;
    public IEnumerable<UnitStateRecord> AttackingUnits { get{ return _attackingUnits; } }

    private readonly IEnumerable<UnitStateRecord> _defendingUnits;
    public IEnumerable<UnitStateRecord> DefendingUnits { get{ return _defendingUnits; } }

    private readonly BattleStatus _status;
    public BattleStatus Status { get{ return _status; } }
    
    public BattleRound(IEnumerable<UnitStateRecord> attackingUnits, IEnumerable<UnitStateRecord> defendingUnits, BattleStatus status)
    {
        _attackingUnits = attackingUnits;
        _defendingUnits = defendingUnits;
        _status = status;
    }
}

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

    public void UpdatePositions(IEnumerable<UnitState> attackers, IEnumerable<UnitState> defenders)
    {
        // TODO: Position Updating System

        throw new NotImplementedException();
    }

    public void Release()
    {
        _unitPositionsBuffer.Release();
        _mapWeightsBuffer.Release();
    }
}