public class TerrorEffector : BattalionEffector
{
    public override BattalionBattleEffects GetEffect(BattalionState self, BattleStageSide allies, BattleStageSide enemies)
    {
        BattalionEffectsBuilder builder = new BattalionEffectsBuilder(this);
        int terrorStrength = self.GetAttribute(BattalionAttribute.TerrorStrength);
        foreach (BattalionState item in enemies)
        {
            builder.Add(item.Id, BattalionAttribute.RemainingMoral, -terrorStrength);
        }
        return builder.ToEffects();
    }
}