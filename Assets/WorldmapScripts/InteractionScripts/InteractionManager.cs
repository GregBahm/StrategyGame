using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionManager
{
    public MapInteraction Map { get; }

    public TimelineInteraction Timeline { get; }

    public TurnMovesProcessor TurnMovesProcessor { get; }

    public ObservableProperty<float> MasterGameTime { get; set; }

    public InteractionManager(MainGameManager mainManager, GameSetup gameSetup, Worldmap worldmap, IEnumerable<PlayerSetup> playerSetups)
    {
        MasterGameTime = new ObservableProperty<float>(0);
        Map = new MapInteraction(gameSetup, worldmap);
        Timeline = new TimelineInteraction(this);
        TurnMovesProcessor = new TurnMovesProcessor(mainManager, playerSetups);
    }

    internal void Update()
    {
        Map.Update();
    }
}
