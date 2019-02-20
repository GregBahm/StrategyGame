using UnityEngine;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    public GameObject MapPrefab;
    public GameObject FactionPrefab;
    public GameObject OrderIndicatorPrefab;
    public MapAssetSetup MapDefinition;
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
