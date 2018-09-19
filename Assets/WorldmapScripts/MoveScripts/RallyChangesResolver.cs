using System;
using System.Collections.Generic;
using System.Linq;

public class RallyChangesResolver
{
    public IEnumerable<ArmyState> AddedArmies { get; }
    /// <summary>
    /// Rally points are changed. New units are received. New armies are create
    /// </summary>
    public GameState NewGameState { get; }

    public RallyChangesResolver(GameState state, List<RallyPointChange> rallyChanges)
    {
        AddedArmies = GetNewArmies();
        IEnumerable<ProvinceState> newProvinces = GetNewProvinces(state, rallyChanges);
        GameState updatedRallyTargets = new GameState(newProvinces, state.Armies);
        NewGameState = ApplyNewUnitsAndArmies(updatedRallyTargets, AddedArmies);
    }

    private IEnumerable<ArmyState> GetNewArmies()
    {
        // TODO: Determine how new armies are created
        return new ArmyState[0];
    }

    private static GameState ApplyNewUnitsAndArmies(GameState gameState, IEnumerable<ArmyState> addedArmies)
    {
        Dictionary<Army, NewArmyUnits> armyTable = gameState.Armies.ToDictionary(item => item.Identifier, item => new NewArmyUnits(item));
        Dictionary<Province, NewProvinceUnits> provinceTable = gameState.Provinces.ToDictionary(item => item.Identifier, item =>new NewProvinceUnits(item));
        foreach (ProvinceState province in gameState.Provinces)
        {
            ArmyForces newForces = province.GetGeneratedArmyForces();
            if(province.RallyTarget.TargetArmyId != null)
            {
                armyTable[province.RallyTarget.TargetArmyId].AddedUnits.Add(newForces);
            }
            else
            {
                provinceTable[province.RallyTarget.TargetProvinceId].NewUnitsToGet.Add(newForces);
            }
        }
        IEnumerable<ProvinceState> updatedProvinces = provinceTable.Values.Select(item => item.GetAfterNewUnits()).ToList();
        List<ArmyState> updatedArmies = armyTable.Values.Select(item => item.GetAfterNewUnits()).ToList();
        updatedArmies.AddRange(addedArmies);
        return new GameState(updatedProvinces, updatedArmies);
    }

    private IEnumerable<ProvinceState> GetNewProvinces(GameState gameState, IEnumerable<RallyPointChange> validChanges)
    {
        HashSet<ProvinceState> ret = new HashSet<ProvinceState>(gameState.Provinces);
        foreach (RallyPointChange change in validChanges)
        {
            ProvinceState oldProvince = gameState.GetProvinceState(change.AlteredProvince);
            ProvinceState newProvince = GetChangedProvince(gameState, oldProvince, change);
            ret.Remove(oldProvince);
            ret.Add(newProvince);
        }
        return ret;
    }

    private ProvinceState GetChangedProvince(GameState state, ProvinceState currentProvince, RallyPointChange change)
    {
        return new ProvinceState(
            currentProvince.Owner,
            currentProvince.Upgrades,
            change.NewRallyTarget,
            currentProvince.Identifier,
            currentProvince.Forces,
            currentProvince.Tiles
            );
    }

    private class NewProvinceUnits
    {
        public ProvinceState BeforeNewUnits { get; }

        public List<ArmyForces> NewUnitsToGet { get; } = new List<ArmyForces>();

        public NewProvinceUnits(ProvinceState beforeNewUnits)
        {
            BeforeNewUnits = beforeNewUnits;
        }

        public ProvinceState GetAfterNewUnits()
        {
            ArmyForces newForces = GetNewForces();

            return new ProvinceState(
                BeforeNewUnits.Owner,
                BeforeNewUnits.Upgrades,
                BeforeNewUnits.RallyTarget,
                BeforeNewUnits.Identifier,
                newForces,
                BeforeNewUnits.Tiles
                );
        }

        private ArmyForces GetNewForces()
        {
            // TODO: Accumulate new forces
            return BeforeNewUnits.Forces;
        }
    }

    private class NewArmyUnits
    {
        public ArmyState BeforeNewUnits { get; }
        
        public List<ArmyForces> AddedUnits { get; } = new List<ArmyForces>();

        public NewArmyUnits(ArmyState beforeNewUnits)
        {
            BeforeNewUnits = beforeNewUnits;
        }

        public ArmyState GetAfterNewUnits()
        {
            ArmyForces newForces = GetNewForces();
            return new ArmyState(
                BeforeNewUnits.Identifier,
                BeforeNewUnits.LocationId,
                 newForces,
                 BeforeNewUnits.Routed
                );

        }

        private ArmyForces GetNewForces()
        {
            // TODO: Accumulate new forces
            return BeforeNewUnits.Forces;
        }
    }
}
