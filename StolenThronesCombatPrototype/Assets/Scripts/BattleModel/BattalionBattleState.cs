using System;
using System.Collections.Generic;
using System.Linq;

public class BattalionBattleState : BattalionState
{
    public int Position { get; }
    public float PresenceWithinRank { get; }
    
    public BattalionBattleState(BattalionIdentifier id,
        IReadOnlyDictionary<BattalionAttribute, int> attributes,
        IEnumerable<BattalionEffector> effectSources,
        List<List<BattalionState>> battleSide)
        :base(id, attributes, effectSources)
    {
        Position = GetPosition(id, battleSide);
        PresenceWithinRank = GetPresenceWithinRank(battleSide);
    }

    private float GetPresenceWithinRank(List<List<BattalionState>> battleSide)
    {
        List<BattalionState> rank = battleSide[Position];
        int totalPresence = rank.Sum(item => item.Presence);
        return (float)Presence / totalPresence;
    }

    private int GetPosition(BattalionIdentifier id, List<List<BattalionState>> battleSide)
    {
        for (int i = 0; i < battleSide.Count; i++)
        {
            if(battleSide[i].Any(item => item.Id == id))
            {
                return i;
            }
        }
        throw new Exception("BattalionState Identifier not present in battleSide");
    }
    
    public IEnumerable<BattalionStateModifier> GetEffects(BattleStateSide allies, BattleStateSide enemies)
    {
        List<BattalionStateModifier> modifiers = new List<BattalionStateModifier>();

        foreach (BattalionEffector effector in EffectSources)
        {
            IEnumerable<BattalionStateModifier> effects = effector.GetEffect(this, allies, enemies);
            modifiers.AddRange(effects);
        }
        return modifiers;
    }
}
