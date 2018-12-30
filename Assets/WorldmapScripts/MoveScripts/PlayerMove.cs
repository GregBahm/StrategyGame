using System;

public abstract class PlayerMove
{
    public enum MoveCategory
    {
        Merger, 
        Upgrade,
        Attack,
    }

    public Faction Faction { get; }
    public MoveCategory Category { get; }

    public PlayerMove(Faction faction, MoveCategory category)
    {
        Faction = faction;
        Category = category;
    }

    public string GetServerMessage()
    {
        throw new NotImplementedException();
    }
    
    public static PlayerMove LoadMoveFromServer(string serverMessage)
    {
        throw new NotImplementedException();
    }
}
