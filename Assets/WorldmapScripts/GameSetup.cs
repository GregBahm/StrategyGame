using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public int Rows = 20;
    public int Columns = 20;
    public GameObject TilePrefab;
    public GameObject ArmyPrefab;
    public GameObject FactionPrefab;
    public Transform MapUvs;

    public Material SkyMat;
    public Canvas ScreenCanvas;

    private MainGameManager _mainManager;

    public UiAethetics Aethetics;

    private void Start()
    {
        _mainManager = new MainGameManager(this);
    }

    private void Update()
    {
        _mainManager.Update(Aethetics);
    }
}
