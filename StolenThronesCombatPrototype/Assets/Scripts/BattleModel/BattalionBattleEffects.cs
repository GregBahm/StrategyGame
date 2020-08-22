using System.Collections.Generic;

public class BattalionBattleEffects
{
    public IEnumerable<BattalionStateModifier> UnitModifications { get; }

    public BattalionBattleEffects(IEnumerable<BattalionStateModifier> unitModifications)
    {
        UnitModifications = unitModifications;
    }
}
