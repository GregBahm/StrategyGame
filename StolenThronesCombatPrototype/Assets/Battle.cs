using System.Collections.Generic;
using System.Linq;

public class Battle
{
    public IReadOnlyCollection<BattleRound> Progression { get; }

    public Battle(BattleStageSide left, BattleStageSide right)
    {
        Progression = GetProgression(left, right).AsReadOnly();
    }

    private List<BattleRound> GetProgression(BattleStageSide left, BattleStageSide right)
    {
        BattleState currentState = new BattleState(left, right);

        List<BattleRound> ret = new List<BattleRound>();
        while (currentState.Status == BattleStatus.Ongoing)
        {
            IEnumerable<BattalionBattleEffects> effects = currentState.GetUnitEffects().ToArray();
            BattleState finalState = currentState.GetWithEffectsApplied(effects);
            BattleRound round = new BattleRound(currentState, effects, finalState);
            ret.Add(round);

            currentState = finalState;
        }
        return ret;
    }
}
