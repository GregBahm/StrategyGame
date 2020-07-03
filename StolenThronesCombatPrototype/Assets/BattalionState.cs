using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattalionState
{
    private static readonly IReadOnlyDictionary<BattalionAttribute, int> DefaultAttributes = new Dictionary<BattalionAttribute, int>
    {
        {BattalionAttribute.MaxHitpoints, 0 },
        {BattalionAttribute.MaxMoral, 0 },
        {BattalionAttribute.RemainingHitpoints, 0 },
        {BattalionAttribute.Armor, 0 },
        {BattalionAttribute.Damage, 0 },
        {BattalionAttribute.Regeneration, 0 },
        {BattalionAttribute.ReloadingSpeed, 0 },
        {BattalionAttribute.ReloadingState, 0 }
    };

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
        IReadOnlyDictionary<BattalionAttribute, int> attributes,
        IEnumerable<BattalionEffector> effectSources)
    {
        Id = id;
        this.attributes = attributes;
        EffectSources = effectSources;
    }

    public BattalionState(BattalionIdentifier id,
        IEnumerable<BattalionStateModifier> modifiers,
        IEnumerable<BattalionEffector> effectSources)
        :this(id, 
             GetModifiedAttributes(DefaultAttributes, modifiers),
             effectSources)
    { }

    public int GetAttribute(BattalionAttribute attribute)
    {
        return attributes[attribute];
    }

    private static Dictionary<BattalionAttribute, int> GetModifiedAttributes(IReadOnlyDictionary<BattalionAttribute, int> attributes, IEnumerable<BattalionStateModifier> modifiers)
    {
        Dictionary<BattalionAttribute, int> newAttributes = new Dictionary<BattalionAttribute, int>(attributes.ToDictionary(item => item.Key, item => item.Value));
        foreach (var item in modifiers)
        {
            newAttributes[item.Attribute] += item.Modifier;
        }
        return newAttributes;
    }

    public BattalionState GetWithModifiersApplied(IEnumerable<BattalionStateModifier> modifiers)
    {
        Dictionary<BattalionAttribute, int> newAttributes = GetModifiedAttributes(attributes, modifiers);
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

    private void SelfModify(Dictionary<BattalionAttribute, int> newAttributes)
    {
        ApplyDamage(newAttributes);
        ApplyHealing(newAttributes);
    }

    private void ApplyHealing(Dictionary<BattalionAttribute, int> newAttributes)
    {
        int hitpoints = newAttributes[BattalionAttribute.RemainingHitpoints];
        if(hitpoints > 0) // Can't heal the dead
        {
            int regeneration = newAttributes[BattalionAttribute.Regeneration];
            int maxHitpoints = newAttributes[BattalionAttribute.MaxHitpoints];
            int newHitpoints = Mathf.Min(maxHitpoints, hitpoints + regeneration);
            newAttributes[BattalionAttribute.RemainingHitpoints] = newHitpoints;
        }
    }

    private void ApplyDamage(Dictionary<BattalionAttribute, int> newAttributes)
    {
        int damage = newAttributes[BattalionAttribute.Damage];
        int apDamage = newAttributes[BattalionAttribute.ArmorPiercingDamage];
        int armor = newAttributes[BattalionAttribute.Armor];
        newAttributes[BattalionAttribute.Damage] = 0;
        int hitpointDamage = Mathf.Max(0, damage - armor - apDamage);
        newAttributes[BattalionAttribute.RemainingHitpoints] -= hitpointDamage;
    }
}
