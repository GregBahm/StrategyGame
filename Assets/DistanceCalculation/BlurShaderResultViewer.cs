using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattlefieldDistancesScript))]
public class BlurShaderResultViewer : MonoBehaviour
{
    public Material AlliesMaterial;
    public Material DefenderMaterial;
    private BattlefieldDistancesScript _main;
    private List<Transform> _transforms;
    private BattlefieldState _currentState;

    public bool MoveUnits;

    private void Start()
    {
        _main = GetComponent<BattlefieldDistancesScript>();
        _currentState = GetCurrentBattlefieldState();
        BuildInitialBoxes(_currentState);
    }

    private void BuildInitialBoxes(BattlefieldState currentState)
    {
        _transforms = new List<Transform>();
        foreach (UnitLocation location in currentState.AttackerPositions)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = new Vector3(location.XPos, 0, location.YPos);
            box.name = "Attacker";
            box.GetComponent<MeshRenderer>().sharedMaterial = AlliesMaterial;
        }
        foreach (UnitLocation location in currentState.DefenderPositions)
        {
            GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.position = new Vector3(location.XPos, 0, location.YPos);
            box.name = "Defender";
            box.GetComponent<MeshRenderer>().sharedMaterial = DefenderMaterial;
        }
        for (int i = 0; i < BattlefieldDistancesScript.HorizontalResolution; i++)
        {
            for (int j = 0; j < BattlefieldDistancesScript.VerticalResolution; j++)
            {
                GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
                box.transform.position = new Vector3(i, 0, j);
                _transforms.Add(box.transform);
            }
        }
    }

    private void Update()
    {
        BattlefieldDistances distances = _main.GetNextState(_currentState);
        DisplayDistanceBoxes(distances);
        //if(MoveUnits)
        //{
        //    MoveUnits = false;
            BattlefieldState nextState = _main.MoveUnits(_currentState, distances);
            _currentState = nextState;
        //}
    }

    private void DisplayDistanceBoxes(BattlefieldDistances distances)
    {
        for (int i = 0; i < BattlefieldDistancesScript.BattlefieldResolution; i++)
        {
            int alliedWeight = distances.Distances[i].AlliedDistance;
            int enemyWeight = distances.Distances[i].EnemyDistance;

            float yPos = alliedWeight / 2 - enemyWeight / 2;
            float yScale = alliedWeight + enemyWeight;
            Transform box = _transforms[i];
            box.transform.position = new Vector3(box.transform.position.x, yPos, box.transform.position.z);
            box.transform.localScale = new Vector3(1, yScale, 1);
        }
    }

    private BattlefieldState GetCurrentBattlefieldState()
    {
        List<UnitLocation> attackerLocations = GetSomeRandomUnitLocations(10);
        List<UnitLocation> defenderLocations = GetSomeRandomUnitLocations(100);
        BattlefieldState ret = new BattlefieldState(attackerLocations.ToArray(),
            defenderLocations.ToArray(),
            new UnitLocation[0],
            new UnitLocation[0]);
        return ret;
    }

    private List<UnitLocation> GetSomeRandomUnitLocations(int xOffset)
    {
        List<UnitLocation> ret = new List<UnitLocation>();
        ret.Add(new UnitLocation(10 + xOffset, 10));
        ret.Add(new UnitLocation(11 + xOffset, 10));
        ret.Add(new UnitLocation(12 + xOffset, 10));
        ret.Add(new UnitLocation(10 + xOffset, 11));
        ret.Add(new UnitLocation(10 + xOffset, 70));
        return ret;
    }
}