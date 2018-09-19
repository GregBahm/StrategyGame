using UnityEngine;

public class GameBindings : MonoBehaviour
{
    public int Rows = 20;
    public int Columns = 20;
    public GameObject TilePrefab;

    [Range(0, 1)]
    public float TileMargin;
    public Transform MapUvs;

    public Material SkyMat;
    public Color BackgroundColor;
    public float HighlightDecaySpeed;

    private MainGameManager _mainManager;
    private UiManager _uiManager;

    public float GameTime;

    private void Start()
    {
        Worldmap worldMap = new Worldmap(TilePrefab, Rows, Columns);
        _mainManager = new MainGameManager(this, worldMap);
        _uiManager = new UiManager(this, _mainManager.WorldMap);
    }

    private void Update()
    {
        GameTime = Mathf.Clamp(GameTime, 0, _mainManager.TurnsCount - 1);
        _mainManager.DisplayGamestate(GameTime);
        _uiManager.Update();
    }
}
