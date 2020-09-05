﻿using System;
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

    internal void InsertState(BattalionBattleState state, BattleRound battleRound, int index)
    {
        BattleState battleState = battleRound.InitialState;
        BattleSideIdentifier side = battleState.GetSide(state.Id);
        BattleStateSide stateSide = side == BattleSideIdentifier.Left ? battleState.LeftSide : battleState.RightSide;
        states[index] = new BattalionStateVisuals(state, side);
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
        int currentHP = state.GetAttribute(BattalionAttribute.RemainingHitpoints);
        return "HP:" + currentHP;
    }

    private void PlaceVisual(BattalionStateVisuals visuals)
    {
        float xPos = visuals.Position + 1;
        xPos *= visuals.Side == BattleSideIdentifier.Left ? -1 : 1;

        primaryVisual.transform.localPosition = new Vector3(xPos, 0, xPos);
        mat.SetFloat("_Flip", visuals.Side == BattleSideIdentifier.Left ? 0 : 1);
    }
}
