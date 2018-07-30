using System;

public class MergerMove : PlayerMove
{
    public Province GrowingProvince { get; }
    public Province AbsorbedProvince { get; }

    public MergerMove(Faction faction, 
        Province growingProvince,
        Province absorbedProvince) 
        : base(faction, MoveCategory.ArmyMove)
    {
        GrowingProvince = growingProvince;
        AbsorbedProvince = absorbedProvince;
    }
    
}
public class UpgradeMove : PlayerMove
{
    public Province TargetProvince { get; }
    public ProvinceUpgrade Upgrade { get; }

    public UpgradeMove(Faction faction,
        Province targetProvince,
        ProvinceUpgrade upgrade) 
        : base(faction, MoveCategory.Upgrade)
    {
        TargetProvince = targetProvince;
        Upgrade = upgrade;
    }
}
public class ArmyMove : PlayerMove
{
    public Army TargetArmy { get; }
    public Province TargetProvince { get; }

    public ArmyMove(Faction faction,
        Army targetArmy,
        Province targetProvince) 
        : base(faction, MoveCategory.ArmyMove)
    {
        TargetArmy = targetArmy;
        TargetProvince = targetProvince;
    }
}
public class RallyPointChange : PlayerMove
{
    public Province TargetProvince { get; }
    public RallyTarget NewRallyTarget { get; }

    public RallyPointChange(Faction faction,
        Province targetProvince,
        RallyTarget newRallyTarget) 
        : base(faction, MoveCategory.RallyPointChange)
    {
        TargetProvince = targetProvince;
        NewRallyTarget = newRallyTarget;
    }
}

public abstract class PlayerMove
{
    public enum MoveCategory
    {
        Merger, 
        Upgrade,
        ArmyMove,
        RallyPointChange,
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
