using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionsHud
{
    public IEnumerable<FactionDisplay> Factions { get; }

    public FactionsHud(GameBindings bindings, IEnumerable<Faction> factions)
    {
        Factions = factions.Select(faction => new FactionDisplay(bindings, faction)).ToArray();
    }
}

public class FactionDisplay
{
    public GameObject GameObject { get; }

    public Faction Faction { get; }

    public FactionDisplay(GameBindings bindings, Faction faction)
    {
        GameObject = GameObject.Instantiate(bindings.FactionPrefab);
        Faction = faction;
    }
}