using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class ProvinceUpgrades
{
    public ReadOnlyCollection<ProvinceUpgrade> Upgrades { get; }

    public ProvinceUpgrades(IEnumerable<ProvinceUpgrade> upgrades)
    {
        Upgrades = upgrades.ToList().AsReadOnly();
    }
}
