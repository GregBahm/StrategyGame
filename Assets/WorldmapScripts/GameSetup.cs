using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public int Rows = 20;
    public int Columns = 20;
    public GameObject TilePrefab;
    public GameObject ArmyPrefab;
    public GameObject FactionPrefab;

    [Range(0, 1)]
    public float TileMargin;
    public Transform MapUvs;

    public Material SkyMat;
    public Color BackgroundColor;
    public float HighlightDecaySpeed;
    public Canvas ScreenCanvas;

    private MainGameManager _mainManager;

    private void Start()
    {
        _mainManager = new MainGameManager(this);
    }

    private void Update()
    {
        _mainManager.Update();
    }
}
