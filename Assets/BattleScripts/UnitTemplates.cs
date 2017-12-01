using System;
public static class UnitTemplates
{
    public static UnitStateBuilder GetSwordsman(int xPos, int yPos)
    {
        string name = "Swordsman";
        string longDescription = "A man with a sword and a shield.";
        UnitDescription description = new UnitDescription(name, longDescription);

        UnitStateBuilder ret = new UnitStateBuilder(description);
        ret.Attributes.Size = 2;
        ret.Attributes.Movement = 4;
        ret.Attributes.HitPoints.Max = 100;
        ret.Attributes.HitPoints.Current = 100;
        ret.Attributes.Moral.Max = 100;
        ret.Attributes.Moral.Current = 100;
        ret.Attributes.Endurance.Max = 100;
        ret.Attributes.Endurance.Current = 100;

        MeleeAttackBuilder swordAttack = new MeleeAttackBuilder();
        swordAttack.AttackPower = 50;
        swordAttack.DamageType = DamageType.Slashing;
        swordAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        ret.MeleeAttacks.Add(swordAttack);

        ret.Attributes.Defense.Armor = 40;
        ret.Attributes.Defense.Shield = ShieldStatus.Medium;

        ret.Attributes.Position.XPos = xPos;
        ret.Attributes.Position.YPos = yPos;

        return ret;
    }

    public static UnitStateBuilder GetArcher(int xPos, int yPos)
    {
        string name = "Archer";
        string longDescription = "A man with a bow and a arrows.";
        UnitDescription description = new UnitDescription(name, longDescription);

        UnitStateBuilder ret = new UnitStateBuilder(description);
        ret.Attributes.Size = 2;
        ret.Attributes.Movement = 4;
        ret.Attributes.HitPoints.Max = 100;
        ret.Attributes.HitPoints.Current = 100;
        ret.Attributes.Moral.Max = 100;
        ret.Attributes.Moral.Current = 80;
        ret.Attributes.Endurance.Max = 80;
        ret.Attributes.Endurance.Current = 100;

        ret.Attributes.Offense.AttackAccuracy = -20;

        MeleeAttackBuilder daggerAttack = new MeleeAttackBuilder();
        daggerAttack.AttackPower = 30;
        daggerAttack.DamageType = DamageType.Piercing;
        daggerAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        ret.MeleeAttacks.Add(daggerAttack);

        RangedAttackBuilder bowAttack = new RangedAttackBuilder();
        bowAttack.Ammunition = 20;
        bowAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        bowAttack.AttackPower = 50;
        bowAttack.MaximumRange = 100;
        bowAttack.MaximumRange = 20;
        ret.RangedAttacks.Add(bowAttack);

        ret.Attributes.Defense.Armor = 20;

        ret.Attributes.Position.XPos = xPos;
        ret.Attributes.Position.YPos = yPos;

        return ret;
    }

    public static UnitStateBuilder GetKnight(int xPos, int yPos)
    {
        string name = "Knight";
        string longDescription = "A knight in shining armor.";
        UnitDescription description = new UnitDescription(name, longDescription);

        UnitStateBuilder ret = new UnitStateBuilder(description);
        ret.Attributes.Size = 3;
        ret.Attributes.Movement = 16;
        ret.Attributes.HitPoints.Max = 100;
        ret.Attributes.HitPoints.Current = 100;
        ret.Attributes.Moral.Max = 200;
        ret.Attributes.Moral.Current = 250;
        ret.Attributes.Endurance.Max = 250;
        ret.Attributes.Endurance.Current = 200;

        ret.Attributes.Offense.AttackAccuracy = 150;
        ret.Attributes.Defense.Dodging = 150;

        MeleeAttackBuilder swordAttack = new MeleeAttackBuilder();
        swordAttack.AttackPower = 50;
        swordAttack.DamageType = DamageType.Slashing;
        swordAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        ret.MeleeAttacks.Add(swordAttack);

        MeleeAttackBuilder lanceAttack = new MeleeAttackBuilder();
        swordAttack.AttackPower = 150;
        swordAttack.DamageType = DamageType.Piercing;
        swordAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        swordAttack.ChargeAttack = true;
        ret.MeleeAttacks.Add(swordAttack);

        ret.Attributes.Defense.Armor = 100;
        ret.Attributes.Defense.Shield = ShieldStatus.Medium;

        ret.Attributes.Position.XPos = xPos;
        ret.Attributes.Position.YPos = yPos;

        return ret;
    }

    public static UnitStateBuilder GetTroll(int xPos, int yPos)
    {
        string name = "Troll";
        string longDescription = "A nasty green troll.";
        UnitDescription description = new UnitDescription(name, longDescription);

        UnitStateBuilder ret = new UnitStateBuilder(description);
        ret.Attributes.Size = 4;
        ret.Attributes.Movement = 3;
        ret.Attributes.HitPoints.Max = 300;
        ret.Attributes.HitPoints.Current = 300;
        ret.Attributes.Moral.Max = 250;
        ret.Attributes.Moral.Current = 250;
        ret.Attributes.Endurance.Max = 200;
        ret.Attributes.Endurance.Current = 200;

        ret.Attributes.Defense.RegenerationPercent = 20;

        MeleeAttackBuilder swipeAttack = new MeleeAttackBuilder();
        swipeAttack.AttackPower = 200;
        swipeAttack.DamageType = DamageType.Slashing;
        swipeAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        ret.MeleeAttacks.Add(swipeAttack);

        ret.Attributes.Defense.Armor = 20;
        
        ret.Attributes.Position.XPos = xPos;
        ret.Attributes.Position.YPos = yPos;

        return ret;
    }

    public static UnitState GetRedMage(int xPos, int yPos)
    {
        throw new NotImplementedException();
    }

    public static UnitState GetGrayMage(int xPos, int yPos)
    {
        throw new NotImplementedException();
    }
}
