using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProvinceDisplay
{
    public Province Identifier { get; }
    private readonly GameDisplayManager _mothership;

    public ProvinceDisplay(Province identifier, GameDisplayManager mothership)
    {
        Identifier = identifier;
        _mothership = mothership;
    }

    public void DisplayProvince(GameTurnTransition transition, DisplayTimings timings)
    {
        DisplayProvinceUpgrades(transition, timings.ProvinceUpgrades);
    }

    private void DisplayProvinceUpgrades(GameTurnTransition transition, float provinceUpgrades)
    {
        ProvinceState initialProvince = transition.BeforeEverything.GetProvinceState(Identifier);
        ProvinceState finalProvince = transition.AfterWarsAndUpgrades.GetProvinceState(Identifier);

    }
}