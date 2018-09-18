using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProvinceDisplay
{
    public Province Identifier { get; }
    public GameDisplayManager _mothership;

    public ProvinceDisplay(Province identifier, GameDisplayManager mothership)
    {
        Identifier = identifier;
        _mothership = mothership;
    }

    public void DisplayProvince(GameTurnTransition transition, DisplayTimings timings)
    {
        ProvinceState initialProvince = transition.InitialState.GetProvinceState(Identifier);
        ProvinceState finalProvince = transition.FinalState.GetProvinceState(Identifier);

        DisplayProvinceEffects(initialProvince, finalProvince, timings.ProvinceEffects);
        DisplayProvinceMergers(initialProvince, finalProvince, timings.ProvinceMergers);
        DisplayProvinceUpgrades(initialProvince, finalProvince, timings.ProvinceUpgrades);
        DisplayProvinceOwnershipChanges(initialProvince, finalProvince, timings.ProvinceOwnershipChanges);
    }

    private void DisplayProvinceOwnershipChanges(ProvinceState initialProvince, ProvinceState finalProvince, float provinceOwnershipChanges)
    {
        throw new NotImplementedException();
    }

    private void DisplayProvinceUpgrades(ProvinceState initialProvince, ProvinceState finalProvince, float provinceUpgrades)
    {
        throw new NotImplementedException();
    }

    private void DisplayProvinceMergers(ProvinceState initialProvince, ProvinceState finalProvince, float provinceMergers)
    {
        throw new NotImplementedException();
    }

    private void DisplayProvinceEffects(ProvinceState initialProvince, ProvinceState finalProvince, float provinceEffects)
    {
        throw new NotImplementedException();
    }
}