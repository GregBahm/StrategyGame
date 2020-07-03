using System;

public static partial class UnitTemplates
{
    public static BattalionState GetSwordsmen()
    {
        BattalionBuilder builder = new BattalionBuilder("Swordsmen",
            100,
            100);
        builder.Set(BattalionAttribute.Armor, 10);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.AddEffector(new MeleeAttackTemplate(10));
        return builder.ToState();
    }

    public static BattalionState GetPeasants()
    {
        BattalionBuilder builder = new BattalionBuilder("Peasants",
            100,
            50);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.AddEffector(new MeleeAttackTemplate(5));
        builder.AddEffector(new RangedAttackTemplate(10));
        return builder.ToState();
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
