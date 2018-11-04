using UnityEngine;

public class MapInteraction
{
    private readonly TileMouseInteraction _tileManager;
    private readonly GameSetup _gameSetup;
    private readonly Worldmap _worldMap;
    private TileDisplay _highlitTile;

    public MapInteraction(GameSetup gameSetup, Worldmap worldMap)
    {
        _gameSetup = gameSetup;
        _worldMap = worldMap;
        _tileManager = new TileMouseInteraction(worldMap);
    }

    public void Update()
    {
        _highlitTile = _tileManager.GetTileUnderMouse();
        Shader.SetGlobalFloat("_TileMargin", _gameSetup.TileMargin);
        Shader.SetGlobalMatrix("_MapUvs", _gameSetup.MapUvs.worldToLocalMatrix);
        Shader.SetGlobalColor("_SideColor", _gameSetup.BackgroundColor);
        _gameSetup.SkyMat.SetColor("_Tint", _gameSetup.BackgroundColor);

        foreach (TileDisplay tile in _worldMap.Tiles)
        {
            tile.UpdateHighlighting(tile == _highlitTile, _gameSetup.HighlightDecaySpeed);
        }
    }
}
