using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class GameState
{
    public ReadOnlyCollection<Province> Provinces { get; }
    public ReadOnlyCollection<Army> Armies { get; }

    public GameState(IEnumerable<Province> provinces, IEnumerable<Army> armies)
    {
        Provinces = provinces.ToList().AsReadOnly();
        Armies = armies.ToList().AsReadOnly();
    }

    public GameTurnTransition GetNextState(GameTurnMoves moves)
    {
        // TODO: Generate new units, apply province effects, and do routing army recovery
        ArmyMovesResolver postArmyMoves = new ArmyMovesResolver(this, moves.ArmyMoves);
        UpgradeMovesResolver postUpgrades = new UpgradeMovesResolver(postArmyMoves.NewGameState, moves.Upgrades);
        MergerMovesResolver postMergers = new MergerMovesResolver(postUpgrades.NewGameState, moves.Mergers);
        RallyChangesResolver postRallyChanges = new RallyChangesResolver(postMergers.NewGameState, moves.RallyChanges);
        // TODO: Move units towards rally points and determine if a player has died
        // TODO: Build the GameTurnTransition
        throw new NotImplementedException();
    }
    
    public Province TryGetEquivalentProvince(Province province)
    {
        return TryGetEquivalentProvince(province.Identifier);
    }
    public Province TryGetEquivalentProvince(Guid provinceId)
    {
        return Provinces.FirstOrDefault(item => item.Identifier == provinceId);
    }

    public Army TryGetEquivalentArmy(Army army)
    {
        return TryGetEquivalentArmy(army.Identifier);
    }
    public Army TryGetEquivalentArmy(Guid armyId)
    {
        return Armies.FirstOrDefault(item => item.Identifier == armyId);
    }
}
