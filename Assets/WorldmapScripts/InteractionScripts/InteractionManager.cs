using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionManager
{
    public MapInteraction Map { get; }

    public FactionsInteractionManager Factions { get; }

    public TimelineInteraction Timeline { get; }

    public InteractionManager(MainGameManager mainManager, 
        GameSetup gameSetup, 
        Map map, 
        UnityObjectManager unityObjectManager,
        IEnumerable<PlayerSetup> playerSetups)
    {
        Map = new MapInteraction(gameSetup, map, mainManager.ObjectManager);
        Timeline = new TimelineInteraction(this);
        Factions = new FactionsInteractionManager(mainManager, unityObjectManager.Factions);
    }

    internal void Update(GameState gameState, ProvinceNeighborsTable neighbors)
    {
        Map.Update(gameState, neighbors);
    }
}
