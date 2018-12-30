using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class GameState
{
    private ReadOnlyDictionary<Tile, ProvinceState> _tileOwners;

    public ReadOnlyCollection<ProvinceState> Provinces { get; }

    public GameState(IEnumerable<ProvinceState> provinces)
    {
        Provinces = provinces.ToList().AsReadOnly();
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
        WarsResolver postWars = new WarsResolver(this, moves.ArmyMoves);
        UpgradeMovesResolver postUpgrades = new UpgradeMovesResolver(postWars.NewGameState, moves.Upgrades);
        MergerMovesResolver postMergers = new MergerMovesResolver(postUpgrades.NewGameState, moves.Mergers);
       
        GameTurnTransition transition = new GameTurnTransition(
            this,
            postWars.NewGameState,
            postUpgrades.NewGameState,
            postMergers.NewGameState,
            postMergers.MergeTable,
            postWars.Wars);

        return transition;
    }
    
    public ProvinceState GetTilesProvince(Tile tile)
    {
        return _tileOwners[tile];
    }
    
    public ProvinceState GetProvinceState(Province provinceId)
    {
        return Provinces.FirstOrDefault(item => item.Identifier == provinceId);
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