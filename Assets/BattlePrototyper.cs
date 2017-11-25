using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePrototyper : MonoBehaviour 
{
    private void Start()
    {
        UnitState swordsmanTemplate = UnitTemplates.GetSwordsman();
        //swordsmanTemplate.Attributes.MaxHitpoints += 50;
        
        UnitState knightTemplate = UnitTemplates.GetKnight();
        UnitState trollTemplate = UnitTemplates.GetTroll();
        UnitState archerTemplate = UnitTemplates.GetArcher();

        List<UnitState> attackingUnits = new List<UnitState>();
        List<UnitState> defendingUnits = new List<UnitState>();
    }
}

public class BattleRound
{
    private readonly IEnumerable<UnitState> _attackingUnits;
    public IEnumerable<UnitState> AttackingUnits;

    private readonly IEnumerable<UnitState> _defendingUnit;
    public IEnumerable<UnitState> DefendingUnits;

    private readonly BattleStatus _status;
    public BattleStatus Status { get{ return _status; } }
    
    public BattleRound(IEnumerable<UnitState> attackingUnits, IEnumerable<UnitState> defendingUnits, BattleStatus status)
    {
        _attackingUnits = attackingUnits;
        _defendingUnit = defendingUnits;
        _status = status;
    }

    public BattleRound GetNextRound()
    {
        throw new NotImplementedException();
    }
}

public enum BattleStatus
{
    Ongoing,
    AttackersVictorious,
    DefendersVictorious,
}

public static class UnitTemplates
{
    public static UnitState GetSwordsman()
    {
        string name = "Swordsman";
        string longDescription = "It's a man with a sword and a shield.";
        UnitDescription description = new UnitDescription(name, longDescription);

        UnitAttributes attributes = new UnitAttributes();
        attributes.Shield = 1;

        MeleeAttackTemplate swordAttack = new MeleeAttackTemplate();
        swordAttack.DamageType = DamageType.Slashing;

        return new UnitState(description, attributes, new[] { swordAttack }, new RangedAttackTemplate[0]);
    }

    public static UnitState GetArcher()
    {
        throw new NotImplementedException();
    }

    public static UnitState GetKnight()
    {
        throw new NotImplementedException();
    }

    public static UnitState GetTroll()
    {
        throw new NotImplementedException();
    }

    public static UnitState GetRedMage()
    {
        throw new NotImplementedException();
    }

    public static UnitState GetGrayMage()
    {
        throw new NotImplementedException();
    }
}

public class UnitState
{
    public readonly UnitAttributes Test;

    private readonly UnitDescription _description;
    public UnitDescription Description { get { return _description; } }

    private readonly UnitAttributes _attributes;
    public UnitAttributes Attributes { get{ return _attributes; } }

    private readonly IEnumerable<MeleeAttackTemplate> _meleeAttacks;
    public IEnumerable<MeleeAttackTemplate> MeleeAttacks { get { return _meleeAttacks; } }

    private readonly IEnumerable<RangedAttackTemplate> _rangedAttacks;
    public IEnumerable<RangedAttackTemplate> RangedAttacks { get { return _rangedAttacks; } }

    public UnitState(UnitDescription description, 
        UnitAttributes attributes,
        IEnumerable<MeleeAttackTemplate> meleAttacks, 
        IEnumerable<RangedAttackTemplate> rangedAttacks)
    {
        _description = description;
        _attributes = attributes;
        _meleeAttacks = meleAttacks;
        _rangedAttacks = rangedAttacks;
    }
}

public struct UnitAttributes
{
    public int XPos { get; set; }
    public int YPos { get; set; }
    public int Size { get; set; }
    public int MaxHitpoints { get; set; }
    public int RemainingHitpoints { get; set; }
    public int MaxEndurance { get; set; }
    public int RemainingEndurance { get; set; }
    public int Movement { get; set; }
    public int Strength { get; set; }
    public int Precision { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int MaxMoral { get; set; }
    public int RemainingMoral { get; set; }

    public int Shield { get; set; }

    public static UnitAttributes GetStandardAttributes()
    {
        int defaultSize = 2;
        int maxHitpoints = defaultSize * defaultSize * 100;
        return new UnitAttributes()
        {
            Size = 2,
            MaxHitpoints = maxHitpoints,
            RemainingHitpoints = maxHitpoints,
            MaxEndurance = 100,
            RemainingEndurance = 100,
            Movement = 1,
            Strength = defaultSize * 5,
            Precision = 100,
            Attack = 100,
            Defense = 100,
            MaxMoral = 100,
            RemainingMoral = 100
        };
    }
}

public class UnitDescription
{
    private readonly string _name;
    public string Name { get { return _name; } }

    private readonly string _longDescription;
    public string LongDescription { get { return _longDescription; } }

    public UnitDescription(string name, string longDescription)
    {
        _name = name;
        _longDescription = longDescription;
    }
}

public struct RangedAttackTemplate
{
    public int AttackPower { get; set; }
    public DamageType DamageType { get; set; }
    public int MaximumRange { get; set; }
    public int MinimumRange{ get; set; }

    public AreaOfEffectType AreaOfEffect { get; set; }
    public int Ammunition { get; set; }
}

public enum AreaOfEffectType
{
    SingleTarget,
    TargetPlusAdject,
    RingAroundUnit,
    TwoByTwo,
}

public enum DamageType
{
    Blunt,
    Piercing,
    Slashing
}

public struct MeleeAttackTemplate
{
    public int AttackPower { get; set; }
    public DamageType DamageType { get; set; }
    public AreaOfEffectType AreaOfEffect { get; set; }
}