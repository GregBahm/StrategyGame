using System;
using TMPro;
using UnityEngine;

public class BattalionVisualizer : MonoBehaviour
{
    private BattalionStateVisuals[] states;

    [SerializeField]
    private GameObject primaryVisual;
    [SerializeField]
    private MeshRenderer meshRenderer;
    private Material mat;

    [SerializeField]
    private TextMeshPro hitpoints;

    [SerializeField]
    private TextMeshPro moral;

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

    private void DisplayState(BattalionStateVisuals visuals)
    {
        if(visuals == null)
        {
            primaryVisual.SetActive(false);
            return;
        }
        primaryVisual.SetActive(true);
        hitpoints.text = GetHPText(visuals.State);
        moral.text = GetMPText(visuals.State);
        PlaceVisual(visuals);
    }

    private string GetMPText(BattalionState state)
    {
        int maxMP = state.GetAttribute(BattalionAttribute.MaxMoral);
        int currentMP = state.GetAttribute(BattalionAttribute.RemainingMoral);
        return "MP:" + currentMP + "\\" + maxMP;
    }

    private string GetHPText(BattalionState state)
    {
        int maxHP = state.GetAttribute(BattalionAttribute.MaxHitpoints);
        int currentHP = state.GetAttribute(BattalionAttribute.RemainingHitpoints);
        return "HP:" + currentHP + "\\" + maxHP;
    }

    private void PlaceVisual(BattalionStateVisuals visuals)
    {
        float xPos = visuals.State.Position.X;
        float sideVal = visuals.Side == BattleSide.Left ? -1 : 1;
        xPos += .5f;
        xPos *= sideVal;
        float zPos = visuals.State.Position.Y;
        zPos -= zPos > 0 ? .5f : -.5f;
        primaryVisual.transform.localPosition = new Vector3(xPos, zPos, 0);
        mat.SetFloat("_Flip", visuals.Side == BattleSide.Left ? 0 : 1);
    }

    private float GetBasePositionValue(BattlePosition position)
    {
        switch (position)
        {
            case BattlePosition.Rear:
                return 3;
            case BattlePosition.Mid:
                return 2;
            case BattlePosition.Front:
            default:
                return 1;
        }
    }
}
