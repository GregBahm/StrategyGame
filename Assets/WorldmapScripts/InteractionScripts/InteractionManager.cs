using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager
{
    public MapInteraction Map { get; }

    public TimelineInteraction Timeline { get; }

    public InteractionManager(GameSetup gameSetup, Worldmap worldmap)
    {
        Map = new MapInteraction(gameSetup, worldmap);
        Timeline = new TimelineInteraction();
    }

    internal void Update()
    {
        Map.Update();
    }
}
