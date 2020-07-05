using System;
using UnityEngine;

public class BattalionVisualizer : MonoBehaviour
{
    private BattalionStateVisuals[] states;

    [SerializeField]
    private GameObject primaryVisual;
    [SerializeField]
    private MeshRenderer meshRenderer;
    private Material mat;
    
    public void Initialize(int totalRounds, Texture2D sprite)
    {
        states = new BattalionStateVisuals[totalRounds];
        mat = meshRenderer.material;
        mat.SetTexture("_MainTex", sprite);
    }

    internal void InsertState(BattalionState state, BattleRound battleRound, int index)
    {
        states[index] = new BattalionStateVisuals(state, battleRound);
    }

    public void Dispay(float normalizedTime)
    {
        int index = (int)(normalizedTime * (states.Length - 1));
        DisplayState(states[index]);
    }

    private void DisplayState(BattalionStateVisuals state)
    {
        if(state == null)
        {
            primaryVisual.SetActive(false);
            return;
        }
        primaryVisual.SetActive(true);
        PlaceVisual(state);
    }

    private void PlaceVisual(BattalionStateVisuals state)
    {
        throw new NotImplementedException();
    }
}
