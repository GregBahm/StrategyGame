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
        DisplayProvinceEffects(transition, timings.ProvinceEffects);
        DisplayProvinceUpgrades(transition, timings.ProvinceUpgrades);
    }

    private void DisplayProvinceUpgrades(GameTurnTransition transition, float provinceUpgrades)
    {
        ProvinceState initialProvince = transition.InitialState.GetProvinceState(Identifier);
        ProvinceState finalProvince = transition.PostUpgradesState.GetProvinceState(Identifier);

    }

    private void DisplayProvinceEffects(GameTurnTransition transition, float provinceEffects)
    {
        ProvinceState initialProvince = transition.InitialState.GetProvinceState(Identifier);
        ProvinceState finalProvince = transition.PostProvinceEffectsState.GetProvinceState(Identifier);

    }
}