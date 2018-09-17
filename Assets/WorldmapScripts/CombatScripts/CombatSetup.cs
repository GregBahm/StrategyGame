using System;
using System.Collections.Generic;
using System.Linq;

public class CombatSetup
{
    public CombatLocation Location {get;}

    public IEnumerable<CombatOutcome> Outcome { get; }

    public CombatSetup(IEnumerable<ArmyMove> movingArmies, IEnumerable<ArmyState> stationaryArmies, ProvinceState defendingProvince)
    {
        // TODO: Sort out how combat rounds are built and resolved
        Location = GetLocation(movingArmies, defendingProvince);
    }

    private static CombatLocation GetLocation(IEnumerable<ArmyMove> movingArmies, ProvinceState defendingProvince)
    {
        if(defendingProvince != null)
        {
            return new CombatLocation(defendingProvince.Identifier);
        }
        Province provinceA = movingArmies.First().Army.LocationId;
        Province provinceB = movingArmies.First(move => move.Army.LocationId != provinceA).Army.LocationId;
        return new CombatLocation(provinceA, provinceB);
    }
}
