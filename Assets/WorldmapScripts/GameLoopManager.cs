using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class MainGameManager
{
    private readonly List<GameTurnTransition> _turns;
    public GameTurnTransition CurrentState { get { return _turns[_turns.Count - 1]; } }

    public MainGameManager()
    {
        _turns = new List<GameTurnTransition>();
    }
    
    public void AdvanceGame(GameTurnMoves moves)
    {
        GameTurnTransition newState = CurrentState.FinalState.GetNextState(moves);
        _turns.Add(newState);
        // TODO: Determine if a player is dead
        // TODO: Update board visuals
    }

    public GameTurnTransition GetTurn(int turn)
    {
        return _turns[turn];
    }
}

public class GameDisplayManager
{
    private readonly List<ArmyDisplay> _armies;
    private readonly List<ProvinceDisplay> _provinces;

    public MainGameManager Game { get; }

    public GameDisplayManager(MainGameManager game)
    {
        _armies = new List<ArmyDisplay>();
        _provinces = new List<ProvinceDisplay>();
        Game = game;
    }
    
    public void DisplayTurn(GameTurnTransition turn, float progression)
    {
        DisplayTimings timings = new DisplayTimings(progression);
        // First new units are generated

        // Then armies move
        UpdateArmies(turn.ArmyTransitions, timings);

        // Then display upgrades
        // Then display mergers
        // Then display rally state changes
        // Then move units towards rally points
    }

    private void UpdateArmies(IEnumerable<ArmyTurnTransition> armyTransitions, DisplayTimings progression)
    {
        Dictionary<Guid, ArmyTurnTransition> transitions = 
            armyTransitions.ToDictionary(transition => transition.StartingState.Identifier, transition => transition);
        foreach (ArmyDisplay displayer in _armies)
        {
            if(transitions.ContainsKey(displayer.Identifier))
            {
                ArmyTurnTransition transition = transitions[displayer.Identifier];
                displayer.DisplayArmy(transition, progression);
            }
            else
            {
                displayer.SetArmyAsDead();
            }
        }
    }
}

public class UnitDisplay
{

}

public class ArmyDisplay
{
    public GameDisplayManager Manager { get; }

    public GameObject ArtContent { get; }

    public Guid Identifier { get; }

    public ArmyDisplay(GameDisplayManager manager, Guid identifier)
    {
        Manager = manager;
        Identifier = identifier;
        ArtContent = MakeArtContent();
    }

    private GameObject MakeArtContent()
    {
        // TODO: Figure out how you're making art content
        throw new NotImplementedException();
    }

    internal void SetArmyAsDead()
    {
        ArtContent.SetActive(false);
    }

    internal void DisplayArmy(ArmyTurnTransition transition, DisplayTimings progression)
    {
        ArtContent.SetActive(true);
        
    }
}

public struct DisplayTimings
{
    public float NewUnits { get; }
    public float ProvinceEffects { get; }
    public float RoutingArmyRecovery { get; }

    public float ArmiesMoveToCollision { get; }
    public float ArmiesDieFromCollisionBattles { get; }
    public float ArmiesToDestination { get; }
    public float ArmiesDieFromNonCollisionBattles { get; }

    public float ProvinceOwnershipChanges { get; }
    public float ProvinceMergers { get; }
    public float ProvinceUpgrades { get; }

    public float RallyChanges { get; }
    public float NewArmyCreated { get; }
    public float UnitsMove { get; }
    public float PlayersDead { get; }

    public DisplayTimings(float turnProgression)
    {

    }
}

public class NewTileDisplay
{
    public GameObject GameObject { get; }

    public Material TileMat { get; }

}

public class ProvinceDisplay
{
    public GUI Identifier { get; }

    public void DisplayProvince(GameState from, GameState to, float param)
    {

    }
}