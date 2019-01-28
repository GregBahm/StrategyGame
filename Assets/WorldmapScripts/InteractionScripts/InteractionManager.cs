using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InteractionManager
{
    private readonly MainGameManager _mainManager;
    public MapInteraction Map { get; }

    public FactionsInteractionManager Factions { get; }

    public TimelineInteraction Timeline { get; }

    public InteractionManager(MainGameManager mainManager, 
        GameSetup gameSetup,
        UnityObjectManager unityObjectManager,
        IEnumerable<PlayerSetup> playerSetups)
    {
        gameSetup.NextTurnButton.onClick.AddListener(() => AdvanceGame());
        _mainManager = mainManager;
        Map = new MapInteraction(this, gameSetup, mainManager.ObjectManager);
        Timeline = new TimelineInteraction(this);
        Factions = new FactionsInteractionManager(unityObjectManager.Factions);
    }

    internal void Update(GameState gameState, ProvinceNeighborsTable neighbors)
    {
        Map.Update(gameState, neighbors);
    }

    public void AdvanceGame()
    {
        IEnumerable<PlayerMove> moves = Factions.Factions.SelectMany(item => item.GetMoves()).ToArray();
        GameTurnMoves turnMoves = new GameTurnMoves(moves);
        _mainManager.AdvanceGame(turnMoves);
    }
}
