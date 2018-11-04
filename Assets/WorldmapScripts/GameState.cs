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

    public IEnumerable<Faction> GetSurvivingFactions()
    {
        IEnumerable<Faction> factions = Provinces.Select(item => item.Owner);
        HashSet<Faction> ret = new HashSet<Faction>(factions);
        return ret;
    }

    public GameTurnTransition GetNextState(GameTurnMoves moves)
    {
        RallyChangesResolver postRallyChanges = new RallyChangesResolver(this, moves.RallyChanges);
        ArmyMovesResolver postArmyMoves = new ArmyMovesResolver(postRallyChanges.NewGameState, moves.ArmyMoves);
        UpgradeMovesResolver postUpgrades = new UpgradeMovesResolver(postArmyMoves.NewGameState, moves.Upgrades);
        MergerMovesResolver postMergers = new MergerMovesResolver(postUpgrades.NewGameState, moves.Mergers);
        // TODO: determine if a player has died
        
        GameState initialState = this;
        GameState postProvinceEffectsState = this; // TODO: Update when you add province effects
        GameState postUpgradesState = postUpgrades.NewGameState;
        GameState postOwnershipChangesState = postArmyMoves.NewGameState;
        GameState postMergersState = postMergers.NewGameState;

        IEnumerable<ArmyTurnTransition> armyTurnTransitions = GetArmyTurnTransitions(postRallyChanges, postArmyMoves);
        
        GameTurnTransition transition = new GameTurnTransition(
            initialState,
            postProvinceEffectsState,
            postUpgradesState,
            postOwnershipChangesState,
            postMergersState,
            postMergers.MergeTable,
            armyTurnTransitions);

        return transition;
    }
    
    public ProvinceState GetTilesProvince(Tile tile)
    {
        return _tileOwners[tile];
    }

    private IEnumerable<ArmyTurnTransition> GetArmyTurnTransitions(RallyChangesResolver rallyChanges, ArmyMovesResolver movesResolver)
    {
        Dictionary<Army, ArmyState> postRallyTable = rallyChanges.NewGameState.Armies.ToDictionary(item => item.Identifier, item => item);
        List<ArmyTurnTransition> ret = new List<ArmyTurnTransition>();
        foreach (ArmyState armyState in Armies)
        {
            ArmyMovesResolver.ArmyStateHistory armyHistory = movesResolver.GetArmyHistory(armyState.Identifier);

            ArmyState postRally = postRallyTable[armyState.Identifier];
            Province armyDestination = movesResolver.GetArmyDestination(armyState.Identifier);
            ArmyState afterCollisionFight = armyHistory.PostCollision;
            ArmyState afterNonCollisionFight = armyHistory.PostNonCollision;
            bool foughtInCollision = armyHistory.FoughtInCollision;
            bool foughtInNonCollision = armyHistory.FoughtInNonCollision;
            ArmyTurnTransition newTrans = new ArmyTurnTransition(
                armyState,
                postRally,
                armyDestination,
                afterCollisionFight,
                afterNonCollisionFight,
                false,
                foughtInCollision,
                foughtInNonCollision);

        }
        foreach (ArmyState armyState in rallyChanges.AddedArmies)
        {
            ArmyMovesResolver.ArmyStateHistory armyHistory = movesResolver.GetArmyHistory(armyState.Identifier);

            bool wasInvaded = armyHistory.FoughtInNonCollision;
            ArmyState postFight = armyHistory.PostNonCollision;
            ArmyTurnTransition newTrans = new ArmyTurnTransition(
                armyState,
                armyState,
                armyState.LocationId,
                armyState,
                postFight,
                true,
                false,
                wasInvaded);
        }
        return ret;
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