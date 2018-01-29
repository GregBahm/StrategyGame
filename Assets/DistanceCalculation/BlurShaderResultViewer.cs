using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BattlefieldDistancesScript))]
public class BlurShaderResultViewer : MonoBehaviour
{
    public Material AlliesMaterial;
    private BattlefieldDistancesScript _main;
    private List<Transform> _transforms;
    private BattlefieldState _currentState;

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
        for (int i = 0; i < BattlefieldDistancesScript.BattlefieldResolution; i++)
        {
            int weight = distances.Distances[i].AlliedDistance;
            Transform box = _transforms[i];
            box.transform.position = new Vector3(box.transform.position.x, (float)weight / 2, box.transform.position.z);
            box.transform.localScale = new Vector3(1, weight, 1);
        }
    }

    private BattlefieldState GetCurrentBattlefieldState()
    {
        BattlefieldState ret = new BattlefieldState();
        List<UnitLocation> allies = GetSomeRandomUnitLocations();
        ret.AttackerPositions = allies.ToArray();
        ret.DefenderPositions = new UnitLocation[0];
        ret.NeutralPositions = new UnitLocation[0];
        ret.BerzerkerPositions = new UnitLocation[0];
        return ret;
    }

    private List<UnitLocation> GetSomeRandomUnitLocations()
    {
        List<UnitLocation> ret = new List<UnitLocation>();
        for (int i = 0; i < 500; i++)
        {
            int randX = UnityEngine.Random.Range(0, BattlefieldDistancesScript.HorizontalResolution - 1);
            int randY = UnityEngine.Random.Range(0, BattlefieldDistancesScript.VerticalResolution - 1);
            UnitLocation newLocation = new UnitLocation(randX, randY);
            ret.Add(newLocation);
        }
        return ret;
    }
}