using UnityEngine;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject FactionPrefab;
    public GameObject OrderIndicatorPrefab;
    public TextAsset MapDefinition;
    public Material SkyMat;
    public Canvas ScreenCanvas;
    public Button NextTurnButton;

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
