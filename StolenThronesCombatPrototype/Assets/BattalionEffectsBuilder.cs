using System.Collections.Generic;

public class BattalionEffectsBuilder
{
    private readonly BattalionEffector source;

    public BattalionEffectsBuilder(BattalionEffector source)
    {
        this.source = source;
    }

    private readonly List<BattalionStateModifier> modifiers = new List<BattalionStateModifier>();
    private readonly List<BattalionSpawnEffect> spawns = new List<BattalionSpawnEffect>();


    public void Add(BattalionIdentifier id, BattalionAttribute attribute, int value)
    {
        modifiers.Add(new BattalionStateModifier(source, id, attribute, value));
    }

    public void Add(BattalionSpawnEffect spawn)
    {
        spawns.Add(spawn);
    }

    public BattalionBattleEffects ToEffects()
    {
        return new BattalionBattleEffects(modifiers, spawns);
    }
}
