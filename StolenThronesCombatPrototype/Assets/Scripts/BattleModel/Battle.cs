using System.Collections.Generic;
using System.Linq;

public class Battle
{
    private const int BattleRoundLimit = 1000;

    public IReadOnlyList<BattleRound> Progression { get; }

    public Battle(BattleStateSide left, BattleStateSide right)
    {
        Progression = GetProgression(left, right).AsReadOnly();
    }

    private List<BattleRound> GetProgression(BattleStateSide left, BattleStateSide right)
    {
        BattleState currentState = new BattleState(left, right);

        List<BattleRound> ret = new List<BattleRound>();
        while (currentState.Status == BattleStatus.Ongoing && ret.Count < BattleRoundLimit)
        {
            IEnumerable<BattalionBattleEffects> effects = currentState.GetUnitEffects().ToArray();
            BattleState withEffectsApplied = currentState.GetWithEffectsApplied(effects);
            BattleState withDefeatedRemoved = withEffectsApplied.GetWithDefeatedRemoved();
            BattleRound round = new BattleRound(currentState, effects, withEffectsApplied, withDefeatedRemoved);
            ret.Add(round);

            currentState = withDefeatedRemoved;
        }
        return ret;
    }
}
