using System;
using System.Linq;

public static class UnitTemplates
{
    public static UnitState GetSwordsman(int xPos, int yPos, UnitAllegiance allegiance)
    {
        string name = "Swordsman";
        string longDescription = "A man with a sword and a shield.";
        UnitIdentification description = new UnitIdentification(name, longDescription);

        UnitState ret = new UnitState(description);
        ret.Size = 2;
        ret.Movement = 4;
        ret.HitPoints.Max = 100;
        ret.Emotions.Moral.Max = 100;
        ret.Emotions.Endurance.Max = 100;

        ret.Defense.Armor = 2;
        ret.Defense.Shield = ShieldStatus.Medium;

        MeleeAttack swordAttack = new MeleeAttack();
        swordAttack.AttackPower = 50;
        swordAttack.DamageType = DamageType.Slashing;
        swordAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        ret.MeleeAttacks.Add(swordAttack);

        ret.HitPoints.Current = ret.HitPoints.Max;
        ret.Emotions.Moral.Current = ret.Emotions.Moral.Max;
        ret.Emotions.Endurance.Current = ret.Emotions.Endurance.Max;

        ret.Position.XPos = xPos;
        ret.Position.YPos = yPos;

        ret.Allegiance = allegiance;

        ValidateTemplate(ret);
        return ret;
    }

    public static UnitState GetArcher(int xPos, int yPos, UnitAllegiance allegiance)
    {
        string name = "Archer";
        string longDescription = "A man with a bow and a arrows.";
        UnitIdentification description = new UnitIdentification(name, longDescription);

        UnitState ret = new UnitState(description);
        ret.Size = 2;
        ret.Movement = 4;
        ret.HitPoints.Max = 100;
        ret.Emotions.Moral.Max = 100;
        ret.Emotions.Endurance.Max = 100;

        ret.Defense.Armor = 1;

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

        ret.HitPoints.Current = ret.HitPoints.Max;
        ret.Emotions.Moral.Current = ret.Emotions.Moral.Max;
        ret.Emotions.Endurance.Current = ret.Emotions.Endurance.Max;

        ret.Position.XPos = xPos;
        ret.Position.YPos = yPos;

        ret.Allegiance = allegiance;

        ValidateTemplate(ret);
        return ret;
    }

    public static UnitState GetKnight(int xPos, int yPos, UnitAllegiance allegiance)
    {
        string name = "Knight";
        string longDescription = "A knight in shining armor.";
        UnitIdentification description = new UnitIdentification(name, longDescription);

        UnitState ret = new UnitState(description);
        ret.Size = 3;
        ret.Movement = 16;
        ret.HitPoints.Max = 100;
        ret.Emotions.Moral.Max = 200;
        ret.Emotions.Endurance.Max = 250;

        ret.Defense.Armor = 3;
        ret.Defense.Shield = ShieldStatus.Medium;

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

        ret.HitPoints.Current = ret.HitPoints.Max;
        ret.Emotions.Moral.Current = ret.Emotions.Moral.Max;
        ret.Emotions.Endurance.Current = ret.Emotions.Endurance.Max;

        ret.Position.XPos = xPos;
        ret.Position.YPos = yPos;

        ret.Allegiance = allegiance;

        ValidateTemplate(ret);
        return ret;
    }

    public static UnitState GetTroll(int xPos, int yPos, UnitAllegiance allegiance)
    {
        string name = "Troll";
        string longDescription = "A nasty green troll.";
        UnitIdentification description = new UnitIdentification(name, longDescription);

        UnitState ret = new UnitState(description);
        ret.Size = 4;
        ret.Movement = 3;
        ret.HitPoints.Max = 300;
        ret.Emotions.Moral.Max = 250;
        ret.Emotions.Endurance.Max = 200;

        ret.Defense.Armor = 0;

        ret.Defense.Regeneration = 20;

        MeleeAttack swipeAttack = new MeleeAttack();
        swipeAttack.AttackPower = 200;
        swipeAttack.DamageType = DamageType.Slashing;
        swipeAttack.AreaOfEffect = AreaOfEffectType.SingleTarget;
        ret.MeleeAttacks.Add(swipeAttack);

        ret.HitPoints.Current = ret.HitPoints.Max;
        ret.Emotions.Moral.Current = ret.Emotions.Moral.Max;
        ret.Emotions.Endurance.Current = ret.Emotions.Endurance.Max;

        ret.Position.XPos = xPos;
        ret.Position.YPos = yPos;

        ret.Allegiance = allegiance;

        ValidateTemplate(ret);
        return ret;
    }

    public static UnitState GetRedMage(int xPos, int yPos, UnitAllegiance allegiance)
    {
        throw new NotImplementedException();
    }

    public static UnitState GetGrayMage(int xPos, int yPos, UnitAllegiance allegiance)
    {
        throw new NotImplementedException();
    }

    private static void ValidateTemplate(UnitState state)
    {
        // Make sure it has size
        if (state.Size < 1)
        {
            throw new Exception(state.ToString() + " has no size");
        }
        // Make sure it has size
        if (state.Movement < 1)
        {
            throw new Exception(state.ToString() + " has no movement");
        }
        // Make sure it has hitpoints
        if (state.HitPoints.Max < 1 || state.HitPoints.Current < 1)
        {
            throw new Exception(state.ToString() + " has no hitpoints");
        }
        // Make sure it has endurance
        if (state.Emotions.Endurance.Max < 1 || state.Emotions.Endurance.Current < 1)
        {
            throw new Exception(state.ToString() + " has no endurance");
        }
        // Make sure it has moral
        if (state.Emotions.Moral.Max < 1 || state.Emotions.Moral.Current < 1)
        {
            throw new Exception(state.ToString() + " has no moral");
        }
        // Make sure it has an attack
        if (state.MeleeAttacks.Any())
        {
            throw new Exception(state.ToString() + " has no melee attacks");
        }
        // If there are ranged attacks, make sure the unit has some precision
        if(state.RangedAttacks.Any())
        {
            if(state.Offense.Precision < 1)
            {
                throw new Exception(state.ToString() + " has ranged attacks but no precision");
            }
            foreach (RangedAttack attack in state.RangedAttacks)
            {
                // Make sure all ranged attacks have some amount of ammo
                if (attack.Ammunition < 1)
                {
                    throw new Exception(attack.ToString() + " of " + state.ToString() + " has no ammunition");
                }
                // Make sure all ranged attacks have a valid range
                if (attack.MaximumRange < 1 || attack.MinimumRange >= attack.MaximumRange)
                {
                    throw new Exception(attack.ToString() + " of " + state.ToString() + " has an invalid range");
                }
            }
        }
    }
}
