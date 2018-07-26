using System;
using System.Collections.Generic;

public class MoveProperties { }

public class PlayerMove<T> 
    where T : MoveProperties 
{
    public enum MoveCategory
    {
        Merger,
        Upgrade,
        ArmyMove,
        RallyPointChange,
    }

    public MoveCategory Category { get; }
    public T Properties { get; }

    public PlayerMove(MoveCategory category, T properties)
    {
        Category = category;
        Properties = properties;
    }

    public string GetServerMessage()
    {

    }
    
}

public class GameLoop
{
    private readonly Faction PlayerFaction;
    private readonly Faction[] AllPlayers;

    public GameLoop()
    {
    }
}
