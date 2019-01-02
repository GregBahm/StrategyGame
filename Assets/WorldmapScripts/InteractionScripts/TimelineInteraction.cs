using System;

public class TimelineInteraction
{
    public float DisplayTime { get; set; }

    private readonly InteractionManager _interactionManager;

    public TimelineInteraction(InteractionManager interactionManager)
    {
        _interactionManager = interactionManager;
    }

    internal void SetToMax(int turnsCount)
    {
        DisplayTime = turnsCount - 1 + .99f;

    }
}
