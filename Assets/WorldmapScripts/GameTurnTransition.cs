using System.Collections.Generic;

public class GameTurnTransition
{
    public GameState BeforeEverything { get; }

    public GameState AfterWars { get; }

    public GameState AfterWarsAndUpgrades { get; }

    public GameState AfterEverything { get; }

    public MergeTable MergeTable { get; }

    public IEnumerable<War> Wars { get; }

    public GameTurnTransition(GameState beforEverything,
        GameState afterWars,
        GameState afterWarsAndUpgrades,
        GameState afterEverything,
        MergeTable mergeTable,
        IEnumerable<War> wars)
    {
        BeforeEverything = beforEverything;
        AfterWars = afterWars;
        AfterWarsAndUpgrades = afterWarsAndUpgrades;
        AfterEverything = afterEverything;
        MergeTable = mergeTable;
        Wars = wars;
    }
}
