using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

public class BattalionState
{
    private readonly IReadOnlyDictionary<BattalionAttribute, int> attributes;

    public BattalionIdentifier Id { get; }

    public bool IsAlive
    {
        get
        {
            return GetAttribute(BattalionAttribute.RemainingHitpoints) > 0
                && GetAttribute(BattalionAttribute.RemainingMoral) > 0;
        }
    }

    public IEnumerable<BattalionEffector> EffectSources { get; }

    private BattalionState(BattalionIdentifier id,
        AttributesTable attributes,
        IEnumerable<BattalionEffector> effectSources)
    {
        Id = id;
        this.attributes = attributes.AsReadonly();
        EffectSources = effectSources;
    }

    public BattalionState(BattalionIdentifier id,
        IEnumerable<BattalionStateModifier> modifiers,
        IEnumerable<BattalionEffector> effectSources)
        : this(id,
             GetModifiedAttributes(new Dictionary<BattalionAttribute, int>(), modifiers),
             effectSources)
    { }

    public int GetAttribute(BattalionAttribute attribute)
    {
        if(attributes.ContainsKey(attribute))
        {
            return attributes[attribute];
        }
        return 0;
    }

    private static AttributesTable GetModifiedAttributes(IReadOnlyDictionary<BattalionAttribute, int> attributes, IEnumerable<BattalionStateModifier> modifiers)
    {
        AttributesTable newAttributes = new AttributesTable(attributes.ToDictionary(item => item.Key, item => item.Value));
        foreach (BattalionStateModifier item in modifiers)
        {
            newAttributes.Add(item.Attribute, item.Modifier);
        }
        return newAttributes;
    }

    public BattalionState GetWithModifiersApplied(IEnumerable<BattalionStateModifier> modifiers)
    {
        AttributesTable newAttributes = GetModifiedAttributes(attributes, modifiers);
        SelfModify(newAttributes);
        return new BattalionState(Id, newAttributes, EffectSources);
    }

    public BattalionBattleEffects GetEffects(BattleStageSide allies, BattleStageSide enemies)
    {
        List<BattalionStateModifier> modifiers = new List<BattalionStateModifier>();
        List<BattalionSpawnEffect> spawns = new List<BattalionSpawnEffect>();

        foreach (BattalionEffector attack in EffectSources)
        {
            var effects = attack.GetEffect(this, allies, enemies);
            modifiers.AddRange(effects.UnitModifications);
            spawns.AddRange(effects.UnitSpawns);

        }
        return new BattalionBattleEffects(modifiers, spawns);
    }

    private void SelfModify(AttributesTable newAttributes)
    {
        ApplyDamage(newAttributes);
        ApplyHealing(newAttributes);
    }

    private void ApplyHealing(AttributesTable newAttributes)
    {
        int hitpoints = newAttributes.Get(BattalionAttribute.RemainingHitpoints);
        if (hitpoints > 0) // Can't heal the dead
        {
            int regeneration = newAttributes.Get(BattalionAttribute.Regeneration);
            int maxHitpoints = newAttributes.Get(BattalionAttribute.MaxHitpoints);
            int newHitpoints = Mathf.Min(maxHitpoints, hitpoints + regeneration);
            newAttributes.Set(BattalionAttribute.RemainingHitpoints, newHitpoints);
        }
    }

    private void ApplyDamage(AttributesTable newAttributes)
    {
        int damage = newAttributes.Get(BattalionAttribute.Damage);
        int apDamage = newAttributes.Get(BattalionAttribute.ArmorPiercingDamage);
        int armor = newAttributes.Get(BattalionAttribute.Armor);
        newAttributes.Set(BattalionAttribute.Damage, 0);
        int hitpointDamage = Mathf.Max(0, damage - armor - apDamage);
        newAttributes.Add(BattalionAttribute.RemainingHitpoints, -hitpointDamage);
    }

    private class AttributesTable
    {
        private readonly Dictionary<BattalionAttribute, int> attributesCore;

        public AttributesTable(Dictionary<BattalionAttribute, int> attributesCore)
        {
            this.attributesCore = new Dictionary<BattalionAttribute, int>(attributesCore);
        }

        public int Get(BattalionAttribute attribute)
        {
            if(attributesCore.ContainsKey(attribute))
            {
                return attributesCore[attribute];
            }
            return 0;
        }

        public void Add(BattalionAttribute attribute, int value)
        {
            if(attributesCore.ContainsKey(attribute))
            {
                attributesCore[attribute] += value;
            }
            else
            {
                attributesCore.Add(attribute, value);
            }
        }

        public void Set(BattalionAttribute attribute, int value)
        {
            if (attributesCore.ContainsKey(attribute))
            {
                attributesCore[attribute] = value;
            }
            else
            {
                attributesCore.Add(attribute, value);
            }
        }

        public IReadOnlyDictionary<BattalionAttribute, int> AsReadonly()
        {
            return new ReadOnlyDictionary<BattalionAttribute, int>(attributesCore);
        }
    }
}
