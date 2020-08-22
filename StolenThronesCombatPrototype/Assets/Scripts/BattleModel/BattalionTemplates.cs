using System;
using UnityEngine;

public class BattalionTemplates
{
    public enum BattalionType
    {
        Swordsmen,
        Slingers,
        Pikemen,
        Catapults,
        Balistas,
        Knights,
        Longbowmen,
        Crossbowmen,
        Dragon,
        Ogres
    }

    public static BattalionState GetSwordsmen()
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Swordsmen,
            100,
            100);
        builder.Set(BattalionAttribute.Armor, 10);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.AddEffector(new MeleeAttackEffector(10));
        return builder.ToState();
    }

    public static BattalionState GetSlinger()
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Slingers,
            100,
            50);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.AddEffector(new MeleeAttackEffector(5));
        builder.AddEffector(new RangedAttackEffector(10, 0));
        return builder.ToState();
    }

    public static BattalionState GetPikemen()
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Pikemen,
            100,
            100);
        builder.Set(BattalionAttribute.Armor, 5);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.Set(BattalionAttribute.AntiCharge, 1);
        builder.Set(BattalionAttribute.MeleeRetribution, 10);
        builder.AddEffector(new MeleeAttackEffector(5));
        return builder.ToState();
    }

    public static BattalionState GetCatapults()
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Catapults,
            50,
            100);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.Set(BattalionAttribute.ReloadingSpeed, 2);
        builder.AddEffector(new MeleeAttackEffector(5));
        builder.AddEffector(new RangedAttackEffector(100, 4));
        return builder.ToState();
    }

    public static BattalionState GetBalista()
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Balistas,
            50,
            100);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.Set(BattalionAttribute.ReloadingSpeed, 2);
        builder.AddEffector(new MeleeAttackEffector(5));
        builder.AddEffector(new RangedAttackEffector(10, 
            4,
            BattalionEffector.DamageType.ArmorPiercing,
            10));
        return builder.ToState();
    }

    public static BattalionState GetKnights()
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Knights,
            100,
            100);
        builder.Set(BattalionAttribute.Armor, 20);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.Set(BattalionAttribute.RallyStrength, 2);
        builder.AddEffector(new MeleeAttackEffector(10, BattalionEffector.MeleeAttackType.Charging));
        builder.AddEffector(new RallyEffector());
        return builder.ToState();
    }

    public static BattalionState GetLongbowmen()
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Longbowmen,
            100,
            100);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.AddEffector(new MeleeAttackEffector(5));
        builder.AddEffector(new RangedAttackEffector(20, 2));
        return builder.ToState();
    }

    public static BattalionState GetCrossbowmen()
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Crossbowmen,
            100,
            100);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.Set(BattalionAttribute.ReloadingSpeed, 1);
        builder.AddEffector(new MeleeAttackEffector(5));
        builder.AddEffector(new RangedAttackEffector(20, 1, damageType: BattalionEffector.DamageType.ArmorPiercing));
        return builder.ToState();
    }

    public static BattalionState GetDragon()
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Dragon,
            1000,
            500);
        builder.Set(BattalionAttribute.Armor, 30);
        builder.Set(BattalionAttribute.Strength, 30);
        builder.Set(BattalionAttribute.TerrorStrength, 2);
        builder.AddEffector(new MeleeAttackEffector(30, BattalionEffector.MeleeAttackType.Charging));
        // TODO: fire fire breath attacks
        builder.AddEffector(new TerrorEffector());
        return builder.ToState();
    }

    public static BattalionState GetOgres()
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Ogres,
            200,
            100);
        builder.Set(BattalionAttribute.Regeneration, 20);
        builder.Set(BattalionAttribute.Strength, 20);
        builder.AddEffector(new MeleeAttackEffector(0));
        builder.AddEffector(new RangedAttackEffector(20, 1));
        return builder.ToState();
    }

    //public static BattalionState GetLeader()
    //public static BattalionState GetAssassin() 
}
