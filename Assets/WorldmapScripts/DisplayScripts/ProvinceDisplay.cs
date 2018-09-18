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

    public Vector3 GetProvinceCenter(GameTurnTransition transition, DisplayTimings timings)
    {
        ProvinceState initialProvinceState = transition.InitialState.Provinces.First(item => item.Identifier == Identifier);
        Vector3 startingCenter = GetAverage(initialProvinceState.Tiles);

        ProvinceState endingProvinceState = transition.FinalState.Provinces.FirstOrDefault(item => item.Identifier == Identifier);
        if (endingProvinceState == null)
        {
            return startingCenter;
        }
        Vector3 endingCenter = GetAverage(endingProvinceState.Tiles);
        Vector3 ret = Vector3.Lerp(startingCenter, endingCenter, timings.ProvinceMergers);
        return ret;
    }

    private Vector3 GetAverage(IEnumerable<Tile> tiles)
    {
        Vector3 ret = Vector3.zero;
        int count = 0;
        foreach (Tile tile in tiles)
        {
            TileDisplay tileDisplay = _mothership.GetTile(tile);
            ret += tileDisplay.GameObject.transform.position;
            count++;
        }
        return ret / count;
    }
}