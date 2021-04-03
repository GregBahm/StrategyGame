using System;
using TMPro;
using UnityEngine;

public class BattalionVisualizer : MonoBehaviour
{
    public BattalionIdentifier Id { get; private set; }
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

    public void Initialize(int totalRounds, Texture2D sprite, BattalionIdentifier id)
    {
        states = new BattalionStateVisuals[totalRounds];
        mat = meshRenderer.material;
        mat.SetTexture("_MainTex", sprite);
        Id = id;
    }

    internal void InsertState(BattalionBattleState state, BattleRound battleRound, int index)
    {
        BattleState battleState = battleRound.InitialState;
        states[index] = new BattalionStateVisuals(state);
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

        //mat.SetFloat("_Flip", visuals.Side == BattleSideIdentifier.Left ? 0 : 1); TODO: Reflipification
    }

    private string GetMPText(BattalionState state)
    {
        int maxMP = state.GetAttribute(BattalionAttribute.MaxMoral);
        int currentMP = state.GetAttribute(BattalionAttribute.RemainingMoral);
        return "MP:" + currentMP + "\\" + maxMP;
    }

    private string GetHPText(BattalionState state)
    {
        int currentHP = state.GetAttribute(BattalionAttribute.RemainingHitpoints);
        return "HP:" + currentHP;
    }
}