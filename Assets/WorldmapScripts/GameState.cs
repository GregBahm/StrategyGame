using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class GameState
{
    private ReadOnlyDictionary<Tile, ProvinceState> _tileOwners;

    public ReadOnlyCollection<ProvinceState> Provinces { get; }
    public ReadOnlyCollection<ArmyState> Armies { get; }

    public GameState(IEnumerable<ProvinceState> provinces, IEnumerable<ArmyState> armies)
    {
        Provinces = provinces.ToList().AsReadOnly();
        Armies = armies.ToList().AsReadOnly();
        _tileOwners = GetTileOwnerDictionary();
    }

    public GameTurnTransition GetNextState(GameTurnMoves moves)
    {
        // TODO: Generate new units, apply province effects, and do routing army recovery
        ArmyMovesResolver postArmyMoves = new ArmyMovesResolver(this, moves.ArmyMoves);
        UpgradeMovesResolver postUpgrades = new UpgradeMovesResolver(postArmyMoves.NewGameState, moves.Upgrades);
        MergerMovesResolver postMergers = new MergerMovesResolver(postUpgrades.NewGameState, moves.Mergers);
        RallyChangesResolver postRallyChanges = new RallyChangesResolver(postMergers.NewGameState, moves.RallyChanges);
        // TODO: Move units towards rally points and determine if a player has died
        IEnumerable<ArmyTurnTransition> armyTurnTransitions = GetArmyTurnTransitions();

        GameState initialState = this;
        GameState postProvinceEffectsState = this; // TODO: Update when you add province effects
        GameState postUpgradesState = postUpgrades.NewGameState;
        GameState postOwnershipChangesState = postArmyMoves.NewGameState;
        GameState postMergersState = postMergers.NewGameState;
        GameState finalState = postRallyChanges.NewGameState;

        GameTurnTransition transition = new GameTurnTransition(
            initialState,
            postProvinceEffectsState,
            postUpgradesState,
            postOwnershipChangesState,
            postMergersState,
            finalState,
            postMergers.MergeTable,
            armyTurnTransitions);

        return transition;
    }

    public ProvinceState GetTilesProvince(Tile tile)
    {
        return _tileOwners[tile];
    }

    private IEnumerable<ArmyTurnTransition> GetArmyTurnTransitions()
    {
        throw new NotImplementedException();
    }

    public ProvinceState GetProvinceState(ProvinceState province)
    {
        return GetProvinceState(province.Identifier);
    }
    public ProvinceState GetProvinceState(Province provinceId)
    {
        return Provinces.FirstOrDefault(item => item.Identifier == provinceId);
    }
    
    public ArmyState GetArmyState(Army armyId)
    {
        return Armies.FirstOrDefault(item => item.Identifier == armyId);
    }
    
    private ReadOnlyDictionary<Tile, ProvinceState> GetTileOwnerDictionary()
    {
        Dictionary<Tile, ProvinceState> ret = new Dictionary<Tile, ProvinceState>();
        foreach (ProvinceState provinceState in Provinces)
        {
            foreach (Tile tile in provinceState.Tiles)
            {
                ret.Add(tile, provinceState);
            }
        }
        return new ReadOnlyDictionary<Tile, ProvinceState>(ret);
    }
}
