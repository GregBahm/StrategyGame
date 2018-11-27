using System;
using UnityEngine;

public class MapInteraction
{
    private readonly TileMouseInteraction _tileManager;
    private readonly GameSetup _gameSetup;
    private readonly Worldmap _worldMap;
    private TileDisplay _highlitTile;
    private int _armyLayerMask;

    public MapInteraction(GameSetup gameSetup, Worldmap worldMap)
    {
        _armyLayerMask =  1 << LayerMask.NameToLayer("ArmyLayer");
        _gameSetup = gameSetup;
        _worldMap = worldMap;
        _tileManager = new TileMouseInteraction(worldMap);
    }

    public void Update()
    {
        SetStandardShaderProperties();

        ArmyDisplay armyHover = GetArmyHover();

        _highlitTile = _tileManager.GetTileUnderMouse();

        bool mouseDown = Input.GetMouseButton(0);
        foreach (TileDisplay tile in _worldMap.Tiles)
        {
            tile.UpdateHighlighting(tile == _highlitTile, _gameSetup.HighlightDecaySpeed, mouseDown);
        }

        if(Input.GetMouseButtonUp(0) && _highlitTile != null)
        {
            // Do something
        }
    }

    private ArmyDisplay GetArmyHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _armyLayerMask, QueryTriggerInteraction.UseGlobal);
        if(hit)
        {
            return hitInfo.transform.gameObject.GetComponent<ArmyDisplayBinding>().ArmyDisplay;
        }
        return null;
    }

    private void SetStandardShaderProperties()
    {
        Shader.SetGlobalFloat("_TileMargin", _gameSetup.TileMargin);
        Shader.SetGlobalMatrix("_MapUvs", _gameSetup.MapUvs.worldToLocalMatrix);
        Shader.SetGlobalColor("_SideColor", _gameSetup.BackgroundColor);
        _gameSetup.SkyMat.SetColor("_Tint", _gameSetup.BackgroundColor);
    }
    
}
