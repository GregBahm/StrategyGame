public class RallyEffector : BattalionEffector
{
    public override BattalionBattleEffects GetEffect(BattalionState self, BattleStageSide allies, BattleStageSide enemies)
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
