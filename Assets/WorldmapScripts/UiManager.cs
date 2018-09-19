using UnityEngine;

public class UiManager
{
    private readonly TileMouseInteraction _tileManager;
    private readonly GameBindings _bindings;
    private readonly Worldmap _worldMap;
    private TileDisplay _highlitTile;

    public UiManager(GameBindings bindings, Worldmap worldMap)
    {
        _bindings = bindings;
        _worldMap = worldMap;
        _tileManager = new TileMouseInteraction(worldMap);
    }

    public void Update()
    {
        _highlitTile = _tileManager.GetTileUnderMouse();
        Shader.SetGlobalFloat("_TileMargin", _bindings.TileMargin);
        Shader.SetGlobalMatrix("_MapUvs", _bindings.MapUvs.worldToLocalMatrix);
        Shader.SetGlobalColor("_SideColor", _bindings.BackgroundColor);
        _bindings.SkyMat.SetColor("_Tint", _bindings.BackgroundColor);

        foreach (TileDisplay tile in _worldMap.Tiles)
        {
            tile.UpdateHighlighting(tile == _highlitTile, _bindings.HighlightDecaySpeed);
        }
    }
}
