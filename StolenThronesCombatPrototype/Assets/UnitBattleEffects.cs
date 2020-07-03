using System.Collections.Generic;

public class BattalionBattleEffects
{
    public IEnumerable<BattalionStateModifier> UnitModifications { get; }
    public IEnumerable<BattalionSpawnEffect> UnitSpawns { get; }

    public BattalionBattleEffects(IEnumerable<BattalionStateModifier> unitModifications, 
        IEnumerable<BattalionSpawnEffect> unitSpawns)
    {
        UnitModifications = unitModifications;
        UnitSpawns = unitSpawns;
    }
}
