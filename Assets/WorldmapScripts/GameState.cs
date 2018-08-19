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

    public GameState GetNextState(GameTurnMoves moves)
    {
        // TODO: Generate new units, apply province effects, and do routing army recovery
        GameState postArmyMovesState = new ArmyMovesResolver(this, moves.ArmyMoves).NewGameState;
        GameState postUpgradesState = new UpgradeMovesResolver(postArmyMovesState, moves.Upgrades).NewGameState;
        GameState postMergersState = new MergerMovesResolver(postUpgradesState, moves.Mergers).NewGameState;
        GameState postRallyChangesState = new RallyChangesResolver(postMergersState, moves.RallyChanges).NewGameState;
        // TODO: Move units towards rally points and determine if a player has died
        return postRallyChangesState;
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
