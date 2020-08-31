using System.Collections.Generic;

public class BattalionEffectsBuilder
{
    private readonly BattalionEffector source;

    public BattalionEffectsBuilder(BattalionEffector source)
    {
        this.source = source;
    }

    private readonly List<BattalionStateModifier> modifiers = new List<BattalionStateModifier>();


    public void Add(BattalionIdentifier id, BattalionAttribute attribute, int value)
    {
        modifiers.Add(new BattalionStateModifier(source, id, attribute, value));
    }

    public IEnumerable<BattalionStateModifier> ToEffects()
    {
        return modifiers;
    }
}
