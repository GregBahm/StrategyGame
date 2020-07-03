
public class MeleeAttackTemplate : BattalionEffector
{
    private readonly int weaponStrength;
    private readonly DamageType damageType;

    public MeleeAttackTemplate(int weaponStrength, DamageType damageType = DamageType.Regular)
    {
        this.weaponStrength = weaponStrength;
        this.damageType = damageType;
    }

    public override BattalionBattleEffects GetEffect(BattalionState self, BattleStageSide allies, BattleStageSide enemies)
    {
        BattalionEffectsBuilder builder = new BattalionEffectsBuilder(this);
        if(allies.GetPosition(self) == BattlePosition.Front)
        {
            BattalionState target = enemies.GetFirstOfRank(BattlePosition.Front);
            int strength = self.GetAttribute(BattalionAttribute.Strength);
            BattalionAttribute damageAttribute = GetDamageAttributeFor(damageType);
            builder.Add(target.Id, damageAttribute, strength + weaponStrength);
        }
        return builder.ToEffects();
    }
}
