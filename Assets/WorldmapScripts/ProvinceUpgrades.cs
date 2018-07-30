using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class ProvinceUpgrades
{
    public ReadOnlyCollection<ProvinceUpgradeBlueprint> Upgrades { get; }

    public ProvinceUpgrades(IEnumerable<ProvinceUpgradeBlueprint> upgrades)
    {
        Upgrades = upgrades.ToList().AsReadOnly();
    }
}
