using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FactionsInteractionManager
{
    private readonly ReadOnlyDictionary<Faction, FactionInteraction> _factions;
    public IEnumerable<FactionInteraction> Factions { get { return _factions.Values; } }
    
    public Faction ActiveFaction { get; private set; }
    public FactionInteraction ActiveInteraction { get { return _factions[ActiveFaction]; } }

    public FactionsInteractionManager(IEnumerable<FactionUnityObject> factionUnities)
    {
        _factions = CreateFactionInteractions(factionUnities);
        ActiveFaction = _factions.First().Value.Faction;
    }

    public FactionInteraction GetFactionInteraction(Faction faction)
    {
        return _factions[faction];
    }

    private ReadOnlyDictionary<Faction, FactionInteraction> CreateFactionInteractions(IEnumerable<FactionUnityObject> objects)
    {
        Dictionary<Faction, FactionInteraction> ret = new Dictionary<Faction, FactionInteraction>();
        foreach (FactionUnityObject obj in objects)
        {
            FactionInteraction interaction = new FactionInteraction(this, obj.Faction);
            obj.gameObject.GetComponent<Button>().onClick.AddListener(() => OnFactionClick(obj.Faction));
            ret.Add(obj.Faction, interaction);
        }
        return new ReadOnlyDictionary<Faction, FactionInteraction>(ret);
    }

    private void OnFactionClick(Faction faction)
    {
        ActiveFaction = faction;
    }

    public void RenewBuilders(IEnumerable<Faction> factions)
    {
        foreach (FactionInteraction builder in Factions)
        {
            builder.Renew();
        }
    }
}
