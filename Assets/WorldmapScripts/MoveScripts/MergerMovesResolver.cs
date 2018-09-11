using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class MergerMovesResolver
{
    public GameState NewGameState { get; }

    public MergerMovesResolver(GameState postArmyMovesState, List<MergerMove> mergers)
    {
        // Need to make sure they're not merging a province they no longer own
        IEnumerable<MergerMove> validMoves = mergers.Where(item => ValidateItem(item, postArmyMovesState));

        // Need to make sure chains of mergers happen in order
        IEnumerable<MergerChain> chains = GetMergerChains(validMoves);
        ProvinceMergerTables merger = new ProvinceMergerTables(postArmyMovesState, chains);
        NewGameState = new GameState(merger.NewProvinces, merger.RedirectedArmies);
    }

    private IEnumerable<MergerChain> GetMergerChains(IEnumerable<MergerMove> validMoves)
    {
        List<MergerChain> chains = new List<MergerChain>();
        foreach (MergerMove move in validMoves)
        {
            MergerChain preChain = chains.FirstOrDefault(chain => chain.Provinces.First() == move.AbsorbedProvince);
            MergerChain postChain = chains.FirstOrDefault(chain => chain.Provinces.Last() == move.GrowingProvince);
            if (preChain != null && postChain != null)
            {
                // Link two chains with this move
                preChain.Provinces.AddRange(postChain.Provinces);
                chains.Remove(postChain);
            }
            else if (preChain != null)
            {
                // Add this move to the end of the pre-chain
                preChain.Provinces.Insert(0, move.GrowingProvince);
            }
            else if (postChain != null)
            {
                // Add this move to the begining of the post-chain
                postChain.Provinces.Add(move.AbsorbedProvince);
            }
            else
            {
                chains.Add(new MergerChain(move));
            }
        }
        return chains;
    }

    private bool ValidateItem(MergerMove merger, GameState state)
    {
        bool growerValid = ValidateProvinceOwner(merger.GrowingProvince, state);
        bool absorberValid = ValidateProvinceOwner(merger.AbsorbedProvince, state);
        return growerValid && absorberValid;
    }

    private bool ValidateProvinceOwner(ProvinceState provinceFromMove, GameState state)
    {
        ProvinceState grower = state.TryGetEquivalentProvince(provinceFromMove);
        return grower.Owner == provinceFromMove.Owner;
    }

    private class MergerChain
    {
        public List<ProvinceState> Provinces { get; }
        public ProvinceState SourceProvince { get { return Provinces.FirstOrDefault(); } }
        public IEnumerable<ProvinceState> EliminatedProvinces { get { return Provinces.Skip(1); } }

        public MergerChain(MergerMove move)
        {
            Provinces = new List<ProvinceState>() { move.GrowingProvince, move.AbsorbedProvince };
        }

        public ProvinceState GetCompletedMerger(GameState state)
        {
            ProvinceState ret = state.TryGetEquivalentProvince(SourceProvince);
            foreach (ProvinceState absorbedProvince in EliminatedProvinces)
            {
                ProvinceState trueAbsorbee = state.TryGetEquivalentProvince(absorbedProvince);
                ret = GetMerged(ret, trueAbsorbee);
            }
            return ret;
        }

        private ProvinceState GetMerged(ProvinceState basis, ProvinceState toAbsorb)
        {
            ProvinceUpgrades newUpgrades = new ProvinceUpgrades(basis.Upgrades.Upgrades.Concat(toAbsorb.Upgrades.Upgrades));
            IEnumerable<Tile> newTiles = basis.Tiles.Concat(toAbsorb.Tiles);
            ProvinceState newProvince = new ProvinceState(
                basis.Owner,
                newUpgrades,
                basis.RallyTarget,
                basis.Identifier,
                newTiles
                );
            return newProvince;
        }
    }

    private class ProvinceMergerTables
    {
        private Dictionary<ProvinceState, ProvinceState> _oldNewDictionary { get; }
        private Dictionary<Guid, ProvinceState> _changesTable { get; }
        public IEnumerable<ProvinceState> NewProvinces { get; }
        public IEnumerable<ArmyState> RedirectedArmies { get; }

        public ProvinceMergerTables(GameState state, IEnumerable<MergerChain> chains)
        {
            _oldNewDictionary = state.Provinces.ToDictionary(item => item, item => item);
            _changesTable = new Dictionary<Guid, ProvinceState>();

            foreach (MergerChain chain in chains)
            {
                ProvinceState mergeChainProduct = chain.GetCompletedMerger(state);
                foreach (ProvinceState province in chain.EliminatedProvinces)
                {
                    ProvinceState toDelete = state.TryGetEquivalentProvince(province);
                    _oldNewDictionary.Remove(toDelete);
                    _changesTable.Add(toDelete.Identifier, mergeChainProduct);
                }
                _oldNewDictionary[chain.SourceProvince] = mergeChainProduct;
            }
            
            NewProvinces = GetNewProvinces();
            RedirectedArmies = GetRedirectedArmies(state.Armies);
        }

        private IEnumerable<ArmyState> GetRedirectedArmies(IEnumerable<ArmyState> oldArmy)
        {
            List<ArmyState> ret = new List<ArmyState>();
            foreach (ArmyState army in oldArmy)
            {
                if(_changesTable.ContainsKey(army.LocationId))
                {
                    ProvinceState newLocation = _changesTable[army.LocationId];
                    ArmyState redirectedArmy = new ArmyState(army.Identifier, newLocation.Identifier, army.Forces, army.Routed);
                    ret.Add(redirectedArmy);
                }
                else
                {
                    ret.Add(army);
                }
            }
            return ret;
        }

        private IEnumerable<ProvinceState> GetNewProvinces()
        {
            List<ProvinceState> ret = new List<ProvinceState>();
            foreach (ProvinceState province in _oldNewDictionary.Values)
            {
                Guid? rallyTarget = province.RallyTarget.TargetProvinceId;
                if(rallyTarget.HasValue && _changesTable.ContainsKey(rallyTarget.Value))
                {
                    RallyTarget newTarget = new RallyTarget(_changesTable[rallyTarget.Value]);
                    ProvinceState newProvince = new ProvinceState(
                        province.Owner,
                        province.Upgrades,
                        newTarget,
                        province.Identifier,
                        province.Tiles);
                    ret.Add(newProvince);
                }
                else
                {
                    ret.Add(province);
                }
            }
            return ret;
        }
    }
}
