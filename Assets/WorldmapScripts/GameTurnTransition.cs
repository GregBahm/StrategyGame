using System.Collections.Generic;

public class GameTurnTransition
{
    /// <summary>
    /// Stage 1 of 5. Before any changes.
    /// </summary>
    public GameState InitialState { get; }
    /// <summary>
    /// Stage 2 of 5. After initial state, before ownership changes. 
    /// </summary>
    public GameState PostProvinceEffectsState { get; }
    /// <summary>
    /// Stage 3 of 5. After province effects, before upgrades.
    /// </summary>
    public GameState PostOwnershipChangesState { get; }
    /// <summary>
    /// Stage 4 of 5. After ownership changes. Before mergers.
    /// </summary>
    public GameState PostUpgradesState { get; }
    /// <summary>
    /// Stage 5 of 5. The final gamestate. Happens after upgrades.
    /// </summary>
    public GameState PostMergersState { get; }
    public MergeTable MergeTable { get; }
    public IEnumerable<ArmyTurnTransition> ArmyTransitions { get; }

    public GameTurnTransition(GameState initialState,
        GameState postProvinceEffectsState,
        GameState postUpgradesState,
        GameState postOwnershipChangesState,
        GameState postMergersState,
        MergeTable mergeTable,
        IEnumerable<ArmyTurnTransition> armyTransitions)
    {
        InitialState = initialState;
        PostProvinceEffectsState = postProvinceEffectsState;
        PostUpgradesState = postUpgradesState;
        PostOwnershipChangesState = postOwnershipChangesState;
        PostMergersState = postMergersState;
        MergeTable = mergeTable;
        ArmyTransitions = armyTransitions;
    }
}
