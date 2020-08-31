using System.Collections.Generic;

public class TerrorEffector : BattalionEffector
{
    public override IEnumerable<BattalionStateModifier> GetEffect(BattalionState self, BattleStateSide allies, BattleStateSide enemies)
    {
        BattalionEffectsBuilder builder = new BattalionEffectsBuilder(this);
        int terrorStrength = self.GetAttribute(BattalionAttribute.TerrorStrength);
        foreach (BattalionState item in enemies.AllUnits)
        {
            builder.Add(item.Id, BattalionAttribute.RemainingMoral, -terrorStrength);
        }
        return builder.ToEffects();
    }
}