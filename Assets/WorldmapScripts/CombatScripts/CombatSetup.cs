using System;
using System.Collections.Generic;
using System.Linq;

public class CombatSetup
{
    public IEnumerable<Army> Participants { get; }

    public CombatLocation Location {get;}

    public IEnumerable<CombatOutcome> Outcome { get; }

    public CombatSetup(GameState state, IEnumerable<ArmyMove> movingArmies, IEnumerable<ArmyState> stationaryArmies, ProvinceState defendingProvince)
    {
        List<Army> participants = new List<Army>();
        participants.AddRange(movingArmies.Select(item => item.Army));
        participants.AddRange(stationaryArmies.Select(item => item.Identifier));
        Participants = participants;

        // TODO: Sort out how combat rounds are built and resolved
        Location = GetLocation(state, movingArmies, defendingProvince);
    }

    private static CombatLocation GetLocation(GameState state, IEnumerable<ArmyMove> movingArmies, ProvinceState defendingProvince)
    {
        if(defendingProvince != null)
        {
            return new CombatLocation(defendingProvince.Identifier);
        }
        Army armyA = movingArmies.First().Army;
        Province provinceA = state.GetArmyState(armyA).LocationId;

        Army armyB = movingArmies.First(move => state.GetArmyState(move.Army).LocationId != provinceA).Army;
        Province provinceB = state.GetArmyState(armyB).LocationId;

        return new CombatLocation(provinceA, provinceB);
    }
}
