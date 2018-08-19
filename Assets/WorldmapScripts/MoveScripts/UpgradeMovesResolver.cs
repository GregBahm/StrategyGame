using System;
using System.Collections.Generic;
using System.Linq;

public class UpgradeMovesResolver
{
    public GameState NewGameState { get; }

    public UpgradeMovesResolver(GameState oldState, List<UpgradeMove> upgrades)
    {
        IEnumerable<UpgradeMove> validUpgrades = upgrades.Where(item => ValidateUpgrade(item, oldState));
        IEnumerable<Province> upgradedProvinces = GetUpgradedProvinces(oldState, validUpgrades);
        NewGameState = new GameState(upgradedProvinces, oldState.Armies);
    }

    private IEnumerable<Province> GetUpgradedProvinces(GameState oldState, IEnumerable<UpgradeMove> validUpgrades)
    {
        Dictionary<Province, Province> oldNewDictionary = oldState.Provinces.ToDictionary(item => item, item => item);
        foreach (UpgradeMove upgrade in validUpgrades)
        {
            Province equivalent = oldState.TryGetEquivalentProvince(upgrade.AlteredProvince);
            Province upgraded = GetUpgradedProvince(upgrade, equivalent);
            oldNewDictionary[equivalent] = upgraded;
        }
        return oldNewDictionary.Values;
    }

    private bool ValidateUpgrade(UpgradeMove item, GameState oldState)
    {
        // Need to make sure the province exists and they're not upgrading a province they no longer own
        Province equivalentProvince = oldState.TryGetEquivalentProvince(item.AlteredProvince);
        return equivalentProvince != null && equivalentProvince.Owner == item.Faction;
    }

    private Province GetUpgradedProvince(UpgradeMove upgrade, Province equivalent)
    {
        // TODO: Apply province upgrades once province upgrades are designed out
        throw new NotImplementedException();
    }
}
