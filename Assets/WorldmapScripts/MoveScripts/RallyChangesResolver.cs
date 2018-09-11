﻿using System;
using System.Collections.Generic;
using System.Linq;

public class RallyChangesResolver
{
    public GameState NewGameState { get; }

    public RallyChangesResolver(GameState state, List<RallyPointChange> rallyChanges)
    {
        IEnumerable<RallyPointChange> validChanges = rallyChanges.Where(item => IsValid(item, state));
        IEnumerable<ProvinceState> newProvinces = GetNewProvinces(state, validChanges);
        NewGameState = new GameState(newProvinces, state.Armies);
    }

    private IEnumerable<ProvinceState> GetNewProvinces(GameState state, IEnumerable<RallyPointChange> validChanges)
    {
        Dictionary<ProvinceState, ProvinceState> oldNewDictionary = state.Provinces.ToDictionary(item => item, item => item);
        foreach (RallyPointChange change in validChanges)
        {
            ProvinceState equivalent = state.TryGetEquivalentProvince(change.AlteredProvince);
            ProvinceState changedProvince = GetChangedProvince(state, equivalent, change);
            oldNewDictionary[equivalent] = changedProvince;
        }
        return oldNewDictionary.Values;
    }

    private ProvinceState GetChangedProvince(GameState state, ProvinceState currentProvince, RallyPointChange change)
    {
        RallyTarget updatedTarget = GetUpdatedTarget(state, change);
        return new ProvinceState(
            currentProvince.Owner,
            currentProvince.Upgrades,
            updatedTarget,
            currentProvince.Identifier,
            currentProvince.Tiles
            );
    }

    private RallyTarget GetUpdatedTarget(GameState state, RallyPointChange change)
    {
        if(change.NewRallyTarget.TargetArmyId.HasValue)
        {
            ArmyState equivalentArmy = state.TryGetEquivalentArmy(change.NewRallyTarget.TargetArmyId.Value);
            return new RallyTarget(equivalentArmy);
        }
        ProvinceState equivalentProvince = state.TryGetEquivalentProvince(change.NewRallyTarget.TargetProvinceId.Value);
        return new RallyTarget(equivalentProvince);
    }

    private bool IsValid(RallyPointChange change, GameState state)
    {
        // Need to make sure they still control the altered province
        ProvinceState sourceEquivalent = state.TryGetEquivalentProvince(change.AlteredProvince);
        if(sourceEquivalent == null || sourceEquivalent.Owner != change.AlteredProvince.Owner)
        {
            return false;
        }

        if (change.NewRallyTarget.TargetProvinceId.HasValue)
        {
            // Need to make sure they still control the target if the target is a province
            ProvinceState targetEquivalent = state.TryGetEquivalentProvince(change.NewRallyTarget.TargetProvinceId.Value);
            return targetEquivalent.Owner == change.Faction;
        }
        else
        {
            // Need to make sure the rally target is still alive if its an army
            ArmyState army = state.TryGetEquivalentArmy(change.NewRallyTarget.TargetArmyId.Value);
            return army != null;
        }
    }
}