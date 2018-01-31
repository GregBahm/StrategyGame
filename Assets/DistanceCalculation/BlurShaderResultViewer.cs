using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlurShaderResultViewer : MonoBehaviour
{
    public ComputeShader Computer;
    public Material AlliesMaterial;
    public Material DefenderMaterial;
    private Battlefield _battlefield;
    private List<Transform> _attackerBoxes;
    private List<Transform> _defenderBoxes;
    private List<Transform> _heightBoxes;
    private BattlefieldState _currentState;

    public bool DoHeightboxes;

    private void Start()
    {
        _battlefield = new Battlefield(Computer);
        _currentState = GetCurrentBattlefieldState();
        BuildUnitBoxes();
        if (DoHeightboxes)
        {
            BuildHeightBoxes();
        }
    }

    private void BuildHeightBoxes()
    {
        _heightBoxes = new List<Transform>();
        for (int i = 0; i < Battlefield.HorizontalResolution; i++)
        {
            for (int j = 0; j < Battlefield.VerticalResolution; j++)
            {
                GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                box.transform.position = new Vector3(i, 0, j);
                _heightBoxes.Add(box.transform);
            }
        }
    }

    private void BuildUnitBoxes()
    {
        _attackerBoxes = new List<Transform>();
        foreach (UnitLocation location in _currentState.AttackerPositions)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = new Vector3(location.XPos, 0, location.YPos);
            box.name = "Attacker";
            box.GetComponent<MeshRenderer>().sharedMaterial = AlliesMaterial;
            _attackerBoxes.Add(box.transform);
        }
        _defenderBoxes = new List<Transform>();
        foreach (UnitLocation location in _currentState.DefenderPositions)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = new Vector3(location.XPos, 0, location.YPos);
            box.name = "Defender";
            box.GetComponent<MeshRenderer>().sharedMaterial = DefenderMaterial;
            _defenderBoxes.Add(box.transform);
        }
    }

    private void Update()
    {
        BattlefieldDistances distances = _battlefield.GetNextState(_currentState);
        BattlefieldState nextState = _battlefield.MoveUnits(_currentState, distances);
        UpdateUnitBoxes(nextState);
        if (DoHeightboxes)
        {
            UpdateHeightBoxes(distances);
        }
        _currentState = nextState;
    }

    private void UpdateUnitBoxes(BattlefieldState nextState)
    {
        for (int i = 0; i < nextState.AttackerPositions.Length; i++)
        {
            UnitLocation attackerPos = nextState.AttackerPositions[i];
            _attackerBoxes[i].position = new Vector3(attackerPos.XPos, 0, attackerPos.YPos);
        }
        for (int i = 0; i < nextState.DefenderPositions.Length; i++)
        {
            UnitLocation defenderPos = nextState.DefenderPositions[i];
            _defenderBoxes[i].position = new Vector3(defenderPos.XPos, 0, defenderPos.YPos);
        }
    }
    private void UpdateHeightBoxes(BattlefieldDistances distances)
    {
        for (int i = 0; i < Battlefield.BattlefieldResolution; i++)
        {
            int alliedWeight = distances.Distances[i].AlliedDistance;
            int enemyWeight = distances.Distances[i].EnemyDistance;

            float yPos = alliedWeight / 2 - enemyWeight / 2;
            float yScale = alliedWeight + enemyWeight;
            Transform box = _heightBoxes[i];
            box.transform.position = new Vector3(box.transform.position.x, yPos, box.transform.position.z);
            box.transform.localScale = new Vector3(1, yScale, 1);
        }
    }

    private BattlefieldState GetCurrentBattlefieldState()
    {
        List<UnitLocation> attackerLocations = GetSomeRandomUnitLocations(10);
        List<UnitLocation> defenderLocations = GetSomeRandomUnitLocations(120);
        BattlefieldState ret = new BattlefieldState(attackerLocations.ToArray(),
            defenderLocations.ToArray(),
            new UnitLocation[0],
            new UnitLocation[0]);
        return ret;
    }

    private List<UnitLocation> GetSomeRandomUnitLocations(int xOffset)
    {
        List<UnitLocation> ret = new List<UnitLocation>();
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                ret.Add(new UnitLocation(i + xOffset, j + 10));
            }
        }
        return ret;
    }

    private void OnDestroy()
    {
        _battlefield.DoDispose();
    }
}