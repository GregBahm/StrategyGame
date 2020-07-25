using System.Collections.Generic;
using System.Linq;

public class Battle
{
    private const int BattleRoundLimit = 1000;

    public IReadOnlyList<BattleRound> Progression { get; }

    public Battle(BattleStageSide left, BattleStageSide right)
    {
        Progression = GetProgression(left, right).AsReadOnly();
    }

    private List<BattleRound> GetProgression(BattleStageSide left, BattleStageSide right)
    {
        BattleState currentState = new BattleState(left, right);

        List<BattleRound> ret = new List<BattleRound>();
        while (ret.Count < BattleRoundLimit)
        {
            IEnumerable<BattalionBattleEffects> effects = currentState.GetUnitEffects().ToArray();
            BattleState withEffectsApplied = currentState.GetWithEffectsApplied(effects);
            if(withEffectsApplied.Status == BattleStatus.Ongoing)
            {
                BattleState withDefeatedRemoved = withEffectsApplied.GetWithDefeatedRemoved();
                BattleRound round = new BattleRound(currentState, effects, withEffectsApplied, withDefeatedRemoved);
                ret.Add(round);
                currentState = withDefeatedRemoved;
            }else
            {
                BattleRound round = new BattleRound(currentState, effects, withEffectsApplied, withEffectsApplied);
                ret.Add(round);
                break;
            }
        }
        return ret;
    }
}
