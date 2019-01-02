using UnityEngine;
using System.Collections;

public class War
{
    public Faction Winner { get; }
    public Province Location { get; }

    public War(Province location, WarForces sideA, WarForces sideB)
    {
        // TOOD: Implement war for realz
        Winner = sideB.Faction;
        Location = location;
    }
}
