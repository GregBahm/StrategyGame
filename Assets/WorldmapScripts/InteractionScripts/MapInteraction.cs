using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapInteraction
{
    private readonly MapUnityObject _mapObject;
    public InteractionManager InteractionMain { get; }
    private readonly int _tileLayermask;
    private readonly ReadOnlyDictionary<int, Tile> _tileByIndex;

    private ProvinceState _hoveredProvince;
    public Province HoveredProvince { get { return _hoveredProvince?.Identifier; } }
    private ProvinceState _selectedProvince;
    public Province SelectedProvince { get { return _selectedProvince?.Identifier; } }
    public bool Dragging { get; private set; }
    private ProvinceState _draggedOnProvince;
    public Province DraggedOnProvince { get { return _draggedOnProvince?.Identifier; } }
    public bool OwnedProvinceSelected
    {
        get
        {
            if(SelectedProvince != null)
            {
                return _selectedProvince.Owner == InteractionMain.Factions.ActiveFaction;
            }
            return false;
        }
    }

    public MapInteraction(InteractionManager main, Map map, UnityObjectManager objectManager)
    {
        InteractionMain = main;
        _tileByIndex = GetTileByIndex(map);
        _tileLayermask = 1 << LayerMask.NameToLayer("UI");
        _mapObject = objectManager.MapObject;
    }

    private ReadOnlyDictionary<int, Tile> GetTileByIndex(IEnumerable<Tile> tiles)
    {
        Dictionary<int, Tile> dictionary = tiles.ToDictionary(item => item.BufferIndex, item => item);
        return new ReadOnlyDictionary<int, Tile>(dictionary);
    }

    public void Update(GameState currentGamestate, ProvinceNeighborsTable neighbors)
    {
        _hoveredProvince = GetProvinceUnderMouse(currentGamestate);
        bool mouseDown = Input.GetMouseButton(0);
        if (mouseDown)
        {
            bool mouseJustDown = Input.GetMouseButtonDown(0);
            if (mouseJustDown)
            {
                HandleHoverToSelected(currentGamestate);
                HandleSelectedToDragging();
            }
            HandleDraggedUpon();
        }
        else
        {
            HandleDragDrop(neighbors);
            _draggedOnProvince = null;
            Dragging = false;
        }
    }

    private void HandleDragDrop(ProvinceNeighborsTable neighbors)
    {
        bool validDrag = GetWasValidDragDrop(neighbors);
        if(validDrag)
        {
            InteractionMain.Factions.ActiveInteraction.RequestAttackOrMerge(_selectedProvince, _draggedOnProvince);
        }
    }

    private bool GetWasValidDragDrop(ProvinceNeighborsTable neighbors)
    {
        bool mouseJustUp = Input.GetMouseButtonUp(0);
        bool wasDragDrop = OwnedProvinceSelected
            && Dragging
            && _draggedOnProvince != null
            && _selectedProvince != _draggedOnProvince
            && mouseJustUp;

        if (wasDragDrop)
        {
            return neighbors.GetNeighborsFor(_selectedProvince.Identifier).Contains(_draggedOnProvince.Identifier);
        }
        return false;
    }

    private void HandleDraggedUpon()
    {
        if (HoveredProvince != null 
            && SelectedProvince != HoveredProvince 
            && Dragging)
        {
            _draggedOnProvince = _hoveredProvince;
        }
        else
        {
            _draggedOnProvince = null;
        }
    }

    private void HandleHoverToSelected(GameState currentGamestate)
    {
        if (HoveredProvince != null)
        {
            _selectedProvince = _hoveredProvince;
        }
    }

    private void HandleSelectedToDragging()
    {
        if (HoveredProvince == SelectedProvince)
        {
            Dragging = true;
        }
    }

    private ProvinceState GetProvinceUnderMouse(GameState currentGamestate)
    {
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            Tile tile = GetTileUnderMouse();
            if (tile != null)
            {
                return currentGamestate.GetTilesProvince(tile);
            }
        }
        return null;
    }

    private Tile GetTileUnderMouse()
    {
        return GetTileAtScreenPoint(Input.mousePosition);
    }

    private static int HexColorToIndex(Color col)
    {
        int x = (int)(col.r * 255);
        int y = (int)(col.g * 255);
        return x + y * 255;
    }

    private Color GetTextureSample(Vector2 textureCoord)
    {
        int xPixel = (int)(textureCoord.x * _mapObject.BaseMap.width);
        int yPixel = (int)(textureCoord.y * _mapObject.BaseMap.height);
        return _mapObject.BaseMap.GetPixel(xPixel, yPixel);
    }

    private Tile GetTileAtScreenPoint(Vector3 pos)
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(mouseRay, out hitInfo, Mathf.Infinity);
        if (hit)
        {
            Color col = GetTextureSample(hitInfo.textureCoord);
            int index = HexColorToIndex(col);
            if(_tileByIndex.ContainsKey(index))
            {
                return _tileByIndex[index];
            }
        }
        return null;
    }
}
