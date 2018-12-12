using System;
using UnityEngine;

public class MapInteraction
{
    private readonly TileMouseInteraction _tileManager;
    private readonly int _armyLayerMask;

    public ArmyDisplay HoveredArmy { get; private set; }
    public ArmyDisplay SelectedArmy { get; private set; }
    public TileDisplay HoveredTile { get; private set; }
    public TileDisplay SelectedTile;

    public MapInteraction(GameSetup gameSetup, Map map, MapDisplay mapDisplay)
    {
        _armyLayerMask =  1 << LayerMask.NameToLayer("ArmyLayer");
        _tileManager = new TileMouseInteraction(mapDisplay);
    }

    public void Update()
    {
        SetHover();
        SetSelected();
    }

    private void SetHover()
    {
        HoveredArmy = GetArmyHover();
        if (HoveredArmy != null)
        {
            HoveredTile = null;
        }
        else
        {
            HoveredTile = _tileManager.GetTileUnderMouse();
        }
    }

    private void SetSelected()
    {
        bool mouseDown = Input.GetMouseButton(0);
        if (mouseDown)
        {
            if(HoveredArmy != null)
            {
                SelectedArmy = HoveredArmy;
            }
            if(HoveredTile != null)
            {
                SelectedTile = HoveredTile;
            }
        }
    }

    private ArmyDisplay GetArmyHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _armyLayerMask, QueryTriggerInteraction.UseGlobal);
        if(hit)
        {
            return hitInfo.transform.parent.gameObject.GetComponent<ArmyDisplayBinding>().ArmyDisplay;
        }
        return null;
    }
    
}
