using System.Collections.Generic;

public class RallyEffector : BattalionEffector
{
    public override IEnumerable<BattalionStateModifier> GetEffect(BattalionState self, BattleStateSide allies, BattleStateSide enemies)
    {
        BattalionEffectsBuilder builder = new BattalionEffectsBuilder(this);
        int rallyStrength = self.GetAttribute(BattalionAttribute.RallyStrength);
        foreach (BattalionState item in allies.AllUnits)
        {
            builder.Add(item.Id, BattalionAttribute.RemainingMoral, rallyStrength);
        }
        return builder.ToEffects();
    }
}
