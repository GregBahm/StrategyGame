using System.Collections.Generic;

public static class UnitTemplates
{
    public static BattalionState GetSwordsmen()
    {
        BattalionIdentifier identifier = new BattalionIdentifier("Swordsmen");
        List<BattalionEffector> effectors = new List<BattalionEffector>()
        { };
        return new BattalionState(identifier,
            10,
            10,
            effectors);
    }

    public static BattalionState GetPeasants()
    {
    }

    public static BattalionState GetPikemen()
    {

    }

    public static BattalionState GetCatapults()
    {

    }

    public static BattalionState GetBalista()
    {

    }

    public static BattalionState GetKnights()
    {
    }

    public static BattalionState GetLongbowmen()
    {
    }

    public static BattalionState GetCrossbowman()
    {
    }

    public static BattalionState GetDragon()
    {
    }

    public static BattalionState GetOgres()
    {
    }
}
