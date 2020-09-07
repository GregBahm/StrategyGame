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

    public static BattalionState GetSwordsmen(int count)
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Swordsmen,
            count,
            100,
            100);
        builder.Set(BattalionAttribute.Defense, 10);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.AddEffector(new MeleeAttackEffector(10));
        return builder.ToState();
    }

    public static BattalionState GetSlingers(int count)
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Slingers,
            count,
            100,
            50);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.AddEffector(new MeleeAttackEffector(5));
        builder.AddEffector(new RangedAttackEffector(10, 0));
        return builder.ToState();
    }

    public static BattalionState GetPikemen(int count)
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Pikemen,
            count,
            100,
            100);
        builder.Set(BattalionAttribute.Defense, 5);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.Set(BattalionAttribute.MeleeRetribution, 10);
        builder.Set(BattalionAttribute.ChargeDefense, 20);
        builder.AddEffector(new MeleeAttackEffector(5));
        return builder.ToState();
    }

    public static BattalionState GetCatapults(int count)
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Catapults,
            count,
            50,
            100);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.Set(BattalionAttribute.ReloadingSpeed, 2);
        builder.AddEffector(new MeleeAttackEffector(5));
        builder.AddEffector(new RangedAttackEffector(100, 4));
        return builder.ToState();
    }

    public static BattalionState GetKnights(int count)
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Knights,
            count,
            100,
            100);
        builder.Set(BattalionAttribute.Defense, 20);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.Set(BattalionAttribute.RallyStrength, 2);
        builder.AddEffector(new MeleeAttackEffector(50, damageType: BattalionEffector.DamageType.Charge));
        builder.AddEffector(new RallyEffector());
        return builder.ToState();
    }

    public static BattalionState GetLongbowmen(int count)
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Longbowmen,
            count,
            100,
            100);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.AddEffector(new MeleeAttackEffector(5));
        builder.AddEffector(new RangedAttackEffector(20, 2));
        return builder.ToState();
    }

    public static BattalionState GetCrossbowmen(int count)
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Crossbowmen,
            count,
            100,
            100);
        builder.Set(BattalionAttribute.Strength, 10);
        builder.Set(BattalionAttribute.ReloadingSpeed, 1);
        builder.AddEffector(new MeleeAttackEffector(5));
        builder.AddEffector(new RangedAttackEffector(20, 1, damageType: BattalionEffector.DamageType.ArmorPiercing));
        return builder.ToState();
    }

    public static BattalionState GetDragon(int count)
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Dragon,
            count,
            1000,
            500);
        builder.Set(BattalionAttribute.Defense, 30);
        builder.Set(BattalionAttribute.Strength, 30);
        builder.Set(BattalionAttribute.TerrorStrength, 2);
        builder.AddEffector(new MeleeAttackEffector(30, damageType: BattalionEffector.DamageType.Charge));
        // TODO: fire fire breath attacks
        builder.AddEffector(new TerrorEffector());
        return builder.ToState();
    }

    public static BattalionState GetOgres(int count)
    {
        BattalionBuilder builder = new BattalionBuilder(BattalionType.Ogres,
            count,
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
