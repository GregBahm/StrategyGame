using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlefieldDistancesScript : MonoBehaviour 
{
    public ComputeShader Computer;

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

    private void OnDestroy()
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
    public UnitLocation GetNextPosition(UnitLocation current, UnitAllegiance alligence, BitArray collisionBitarray, BattlefieldDistances distances)
    {
        IEnumerable<UnitLocation> adjacent = AdjacencyFinder.GetAdjacentPositions(current.XPos, current.YPos, 1);
        IEnumerable<UnitLocation> notBlocked = adjacent.Where(item => !PositionOccupied(item, collisionBitarray));
        IEnumerable<DistanceCheck> distanceChecks = notBlocked.Select(item => GetDistanceCheck(item, distances, alligence));
        if(!distanceChecks.Any())
        {
            return current;
        }
        int bestDistance = distanceChecks.Min(item => item.Distance);
        IEnumerable<DistanceCheck> bestDistances = distanceChecks.Where(item => item.Distance == bestDistance);

        // TODO: Prioritize movement away from friends
        return bestDistances.First().Location;
    }

    private DistanceCheck GetDistanceCheck(UnitLocation item, BattlefieldDistances distances,  UnitAllegiance alligence)
    {
        BattlefieldDistance dist = distances.GetDistanceAt(item.XPos, item.YPos);
        int amount;
        switch (alligence)
        {
            case UnitAllegiance.Attackers:
                amount = dist.EnemyDistance;
                break;
            case UnitAllegiance.Defenders:
                amount = dist.AlliedDistance;
                break;
            case UnitAllegiance.Neutrals:
                amount = dist.NeutralDistance;
                break;
            case UnitAllegiance.AttacksAll:
            default:
                amount = dist.BerzerkerDistance;
                break;
        }
        return new DistanceCheck(amount, item);
    }

    private static bool PositionOccupied(UnitLocation position, BitArray collisionBitarray)
    {
        int index = BattlefieldDistancesScript.UvToIndex(position);
        return collisionBitarray[index];
    }

    private struct DistanceCheck
    {
        public int Distance;
        public UnitLocation Location;

        public DistanceCheck(int distance, UnitLocation location)
        {
            Distance = distance;
            Location = location;
        }
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
    private readonly UnitLocation[] _attackerPositions;
    public UnitLocation[] AttackerPositions { get{ return _attackerPositions; } }
    private readonly UnitLocation[] _defenderPositions;
    public UnitLocation[] DefenderPositions { get{ return _defenderPositions; } }
    private readonly UnitLocation[] _neutralPositions;
    public UnitLocation[] NeutralPositions { get{ return _neutralPositions; } }
    private readonly UnitLocation[] _berzerkerPositions;
    public UnitLocation[] BerzerkerPositions { get{ return _berzerkerPositions; } }

    public IEnumerable<UnitLocation> AllUnits
    {
        get
        {
            foreach (UnitLocation dude in _attackerPositions)
            {
                yield return dude;
            }
            foreach (UnitLocation dude in _defenderPositions)
            {
                yield return dude;
            }
            foreach (UnitLocation dude in _neutralPositions)
            {
                yield return dude;
            }
            foreach (UnitLocation dude in _berzerkerPositions)
            {
                yield return dude;
            }
        }
    }

    public BattlefieldState(UnitLocation[] attackerPositions,
        UnitLocation[] defenderPositions,
        UnitLocation[] neutralPositions,
        UnitLocation[] berzerkerPositions)
    {
        _attackerPositions = attackerPositions;
        _defenderPositions = defenderPositions;
        _neutralPositions = neutralPositions;
        _berzerkerPositions = berzerkerPositions;
    }
}
