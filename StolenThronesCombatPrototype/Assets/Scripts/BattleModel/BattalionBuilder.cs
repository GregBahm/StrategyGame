﻿using System.Collections.Generic;
using static BattalionTemplates;

public class BattalionBuilder
{
    private readonly BattalionIdentifier id;
    private readonly List<BattalionEffector> effectors;
    private readonly List<BattalionStateModifier> modifiers;

    public BattalionBuilder(BattalionType type,
        int unitCount,
        int hitpointsPerUnit,
        int moral)
    {
        id = new BattalionIdentifier(type);
        effectors = new List<BattalionEffector>();
        modifiers = new List<BattalionStateModifier>();

        modifiers.Add(new BattalionStateModifier(null, id, BattalionAttribute.HitpointsPerUnit, hitpointsPerUnit));
        modifiers.Add(new BattalionStateModifier(null, id, BattalionAttribute.RemainingHitpoints, hitpointsPerUnit * unitCount));
        modifiers.Add(new BattalionStateModifier(null, id, BattalionAttribute.MaxUnits, unitCount));
        modifiers.Add(new BattalionStateModifier(null, id, BattalionAttribute.MaxMoral, moral));
        modifiers.Add(new BattalionStateModifier(null, id, BattalionAttribute.RemainingMoral, moral));
    }

    public void Set(BattalionAttribute attribute, int value)
    {
        this.modifiers.Add(new BattalionStateModifier(null, id, attribute, value));
    }

    public void AddEffector(BattalionEffector effector)
    {
        this.effectors.Add(effector);
    }

    public BattalionState ToState()
    {
        return new BattalionState(id,
            modifiers,
            effectors);
    }
}