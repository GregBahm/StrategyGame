using System;
using UnityEngine;

[Serializable]
public class UiAethetics
{
    public Color HoverColor;
    public Color SelectedColor;
    public Color DraggingColor;
    public Color SelectingColor;
    public float TransitionSpeed;

    public Color BackgroundColor;
    [Range(0, 1)]
    public float TileMargin;
}