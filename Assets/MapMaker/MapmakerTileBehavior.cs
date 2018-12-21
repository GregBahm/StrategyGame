using System;
using UnityEngine;

public class MapmakerTileBehavior : MonoBehaviour
{
    private bool _lastStartPos;
    public bool IsStartPosition;

    private bool _lastImpassable;
    public bool IsImpassable;

    public TileRotationSet RotationSet;

    private void Update()
    {
        if(IsStartPosition && _lastImpassable)
        {
            _lastImpassable = false;
            IsImpassable = false;
        }
        if(IsImpassable && _lastStartPos)
        {
            _lastStartPos = false;
            IsStartPosition = false;
        }
        if(_lastStartPos != IsStartPosition || _lastImpassable != IsImpassable)
        {
            RotationSet.Main.UpdateStartingLocations();
            UpdateMaterials();
        }
        _lastImpassable = IsImpassable;
        _lastStartPos = IsStartPosition;
    }

    private void UpdateMaterials()
    {
        RotationSet.UpdateMaterials();
    }
}
