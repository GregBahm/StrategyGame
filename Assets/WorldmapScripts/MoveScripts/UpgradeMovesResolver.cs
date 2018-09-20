using System;
using System.Collections.Generic;
using System.Linq;

public class UpgradeMovesResolver
{
    public GameState NewGameState { get; }

    public UpgradeMovesResolver(GameState oldState, List<UpgradeMove> upgrades)
    {
        IEnumerable<UpgradeMove> validUpgrades = upgrades.Where(item => ValidateUpgrade(item, oldState));
        IEnumerable<ProvinceState> upgradedProvinces = GetUpgradedProvinces(oldState, validUpgrades);
        NewGameState = new GameState(upgradedProvinces, oldState.Armies);
    }

    private IEnumerable<ProvinceState> GetUpgradedProvinces(GameState oldState, IEnumerable<UpgradeMove> validUpgrades)
    {
        Dictionary<Province, ProvinceState> oldNewDictionary = oldState.Provinces.ToDictionary(item => item.Identifier, item => item);
        foreach (UpgradeMove upgrade in validUpgrades)
        {
            ProvinceState provinceState = oldState.GetProvinceState(upgrade.AlteredProvince);
            ProvinceState upgraded = GetUpgradedProvince(upgrade, provinceState);
            oldNewDictionary[upgrade.AlteredProvince] = upgraded;
        }
        return oldNewDictionary.Values;
    }

    private bool ValidateUpgrade(UpgradeMove item, GameState oldState)
    {
        // Need to make sure the province exists and they're not upgrading a province they no longer own
        ProvinceState province = oldState.GetProvinceState(item.AlteredProvince);
        return province != null && province.Owner == item.Faction;
    }

    private ProvinceState GetUpgradedProvince(UpgradeMove upgrade, ProvinceState equivalent)
    {
        // TODO: Apply province upgrades once province upgrades are designed out
        throw new NotImplementedException();
    }
}
