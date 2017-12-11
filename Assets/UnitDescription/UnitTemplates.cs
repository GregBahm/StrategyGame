using System;
public static class UnitTemplates
{
    public static UnitState GetSwordsman(int xPos, int yPos)
    {
        string name = "Swordsman";
        string longDescription = "A man with a sword and a shield.";
        UnitIdentification description = new UnitIdentification(name, longDescription);

        UnitState ret = new UnitState(description);
        ret.Size = 2;
        ret.Movement = 4;
        ret.HitPoints.Max = 100;
        ret.HitPoints.Current = 100;
        ret.Emotions.Moral.Max = 100;
        ret.Emotions.Moral.Current = 100;
        ret.Emotions.Endurance.Max = 100;
        ret.Emotions.Endurance.Current = 100;

        MeleeAttack swordAttack = new MeleeAttack();
        swordAttack.AttackPower = 50;
        swordAttack.DamageType = DamageType.Slashing;
        swordAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        ret.MeleeAttacks.Add(swordAttack);

        ret.Defense.Armor = 40;
        ret.Defense.Shield = ShieldStatus.Medium;

        ret.Position.XPos = xPos;
        ret.Position.YPos = yPos;

        return ret;
    }

    public static UnitState GetArcher(int xPos, int yPos)
    {
        string name = "Archer";
        string longDescription = "A man with a bow and a arrows.";
        UnitIdentification description = new UnitIdentification(name, longDescription);

        UnitState ret = new UnitState(description);
        ret.Size = 2;
        ret.Movement = 4;
        ret.HitPoints.Max = 100;
        ret.HitPoints.Current = 100;
        ret.Emotions.Moral.Max = 100;
        ret.Emotions.Moral.Current = 80;
        ret.Emotions.Endurance.Max = 80;
        ret.Emotions.Endurance.Current = 100;

        ret.Offense.AttackAccuracy = -20;

        MeleeAttack daggerAttack = new MeleeAttack();
        daggerAttack.AttackPower = 30;
        daggerAttack.DamageType = DamageType.Piercing;
        daggerAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        ret.MeleeAttacks.Add(daggerAttack);

        RangedAttack bowAttack = new RangedAttack();
        bowAttack.Ammunition = 20;
        bowAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        bowAttack.AttackPower = 50;
        bowAttack.MaximumRange = 100;
        bowAttack.MaximumRange = 20;
        ret.RangedAttacks.Add(bowAttack);

        ret.Defense.Armor = 20;

        ret.Position.XPos = xPos;
        ret.Position.YPos = yPos;

        return ret;
    }

    public static UnitState GetKnight(int xPos, int yPos)
    {
        string name = "Knight";
        string longDescription = "A knight in shining armor.";
        UnitIdentification description = new UnitIdentification(name, longDescription);

        UnitState ret = new UnitState(description);
        ret.Size = 3;
        ret.Movement = 16;
        ret.HitPoints.Max = 100;
        ret.HitPoints.Current = 100;
        ret.Emotions.Moral.Max = 200;
        ret.Emotions.Moral.Current = 250;
        ret.Emotions.Endurance.Max = 250;
        ret.Emotions.Endurance.Current = 200;

        ret.Offense.AttackAccuracy = 150;
        ret.Defense.Dodging = 150;

        MeleeAttack swordAttack = new MeleeAttack();
        swordAttack.AttackPower = 50;
        swordAttack.DamageType = DamageType.Slashing;
        swordAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        ret.MeleeAttacks.Add(swordAttack);

        MeleeAttack lanceAttack = new MeleeAttack();
        swordAttack.AttackPower = 150;
        swordAttack.DamageType = DamageType.Piercing;
        swordAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        swordAttack.ChargeAttack = true;
        ret.MeleeAttacks.Add(swordAttack);

        ret.Defense.Armor = 100;
        ret.Defense.Shield = ShieldStatus.Medium;

        ret.Position.XPos = xPos;
        ret.Position.YPos = yPos;

        return ret;
    }

    public static UnitState GetTroll(int xPos, int yPos)
    {
        string name = "Troll";
        string longDescription = "A nasty green troll.";
        UnitIdentification description = new UnitIdentification(name, longDescription);

        UnitState ret = new UnitState(description);
        ret.Size = 4;
        ret.Movement = 3;
        ret.HitPoints.Max = 300;
        ret.HitPoints.Current = 300;
        ret.Emotions.Moral.Max = 250;
        ret.Emotions.Moral.Current = 250;
        ret.Emotions.Endurance.Max = 200;
        ret.Emotions.Endurance.Current = 200;

        ret.Defense.RegenerationPercent = 20;

        MeleeAttack swipeAttack = new MeleeAttack();
        swipeAttack.AttackPower = 200;
        swipeAttack.DamageType = DamageType.Slashing;
        swipeAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        ret.MeleeAttacks.Add(swipeAttack);

        ret.Defense.Armor = 20;
        
        ret.Position.XPos = xPos;
        ret.Position.YPos = yPos;

        return ret;
    }

    public static UnitStateRecord GetRedMage(int xPos, int yPos)
    {
        throw new NotImplementedException();
    }

    public static UnitStateRecord GetGrayMage(int xPos, int yPos)
    {
        throw new NotImplementedException();
    }
}
