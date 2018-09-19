﻿using System.Collections.Generic;

public class GameTurnTransition
{
    public GameState InitialState { get; }
    public GameState PostProvinceEffectsState { get; }
    public GameState PostOwnershipChangesState { get; }
    public GameState PostUpgradesState { get; }
    public GameState PostMergersState { get; }
    public GameState FinalState { get; }
    public MergeTable MergeTable { get; }
    public IEnumerable<ArmyTurnTransition> ArmyTransitions { get; }

    public GameTurnTransition(GameState initialState,
        GameState postProvinceEffectsState,
        GameState postUpgradesState,
        GameState postOwnershipChangesState,
        GameState postMergersState,
        GameState finalState,
        MergeTable mergeTable,
        IEnumerable<ArmyTurnTransition> armyTransitions)
    {
        InitialState = initialState;
        PostProvinceEffectsState = postProvinceEffectsState;
        PostUpgradesState = postUpgradesState;
        PostOwnershipChangesState = postOwnershipChangesState;
        PostMergersState = postMergersState;
        FinalState = finalState;
        MergeTable = mergeTable;
        ArmyTransitions = armyTransitions;
    }
}
