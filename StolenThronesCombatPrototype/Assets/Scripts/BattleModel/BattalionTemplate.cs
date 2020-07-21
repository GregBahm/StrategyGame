using System.Collections.Generic;

public class BattalionTemplate
{
    public BattalionIdentifier Id { get; }
    public IEnumerable<BattalionStateModifier> Modifiers { get; }
    public IEnumerable<BattalionEffector> EffectSources { get; }

    public BattalionTemplate(BattalionIdentifier id,
        IEnumerable<BattalionStateModifier> modifiers,
        IEnumerable<BattalionEffector> effectSources)
    {
        Id = id;
        Modifiers = modifiers;
        EffectSources = effectSources;
    }

    public BattalionState ToState(int xPos, int yPos)
    {
        return new BattalionState(Id,
            new BattalionPosition(xPos, yPos),
            Modifiers,
            EffectSources);
    }
}
