using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

        UnitAttributesBuilder attributes = new UnitAttributesBuilder();
        attributes.Shield = 1;

        MeleeAttack swordAttack = new MeleeAttack();
        swordAttack.DamageType = DamageType.Slashing;

        return new UnitState(description, attributes, new[] { swordAttack }, new RangedAttack[0]);
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
    private readonly UnitDescription _description;
    public UnitDescription Description { get { return _description; } }

    private readonly UnitAttributes _attributes;
    public UnitAttributes Attributes { get{ return _attributes; } }

    private readonly IEnumerable<MeleeAttack> _meleeAttacks;
    public IEnumerable<MeleeAttack> MeleeAttacks { get { return _meleeAttacks; } }

    private readonly IEnumerable<RangedAttack> _rangedAttacks;
    public IEnumerable<RangedAttack> RangedAttacks { get { return _rangedAttacks; } }

    public UnitState(UnitDescription description, 
        UnitAttributes attributes,
        IEnumerable<MeleeAttack> meleAttacks, 
        IEnumerable<RangedAttack> rangedAttacks)
    {
        _description = description;
        _attributes = attributes;
        _meleeAttacks = meleAttacks;
        _rangedAttacks = rangedAttacks;
    }
}

public class UnitStateBuilder
{
    private readonly UnitDescription _description;
    public UnitDescription Description { get { return _description; } }

    private readonly UnitAttributesBuilder _attributes;
    public UnitAttributesBuilder Attributes { get { return _attributes; } }

    private readonly List<MeleeAttackBuilder> _meleeAttacks;
    public List<MeleeAttackBuilder> MeleeAttacks { get { return _meleeAttacks; } }

    private readonly List<RangedAttackBuilder> _rangedAttacks;
    public List<RangedAttackBuilder> RangedAttacks { get { return _rangedAttacks; } }

    public UnitStateBuilder(UnitDescription description,
        UnitAttributesBuilder attributes,
        List<MeleeAttackBuilder> meleAttacks,
        List<RangedAttackBuilder> rangedAttacks)
    {
        _description = description;
        _attributes = attributes;
        _meleeAttacks = meleAttacks;
        _rangedAttacks = rangedAttacks;
    }

    public UnitState ToReadonly()
    {
        return new UnitState(Description,
            Attributes.ToReadonly(),
            MeleeAttacks.Select(item => item.ToReadonly()).ToArray(),
            RangedAttacks.Select(item => item.ToReadonly()).ToArray())
    }
}

public struct UnitAdvancedDefenses
{
    private readonly ShieldStatus _shield;
    public ShieldStatus Shield { get { return _shield; } }

    private readonly int _regenerationPercent;
    public int RegenerationPercent { get{ return _regenerationPercent; } }

    public UnitAdvancedDefenses(ShieldStatus shield, int regenerationPercent)
    {
        _shield = shield;
        _regenerationPercent = regenerationPercent;
    }
}

public class UnitAdvancedDefensesBuilder
{
    public ShieldStatus Shield { get; set; }

    public int RegenerationPercent { get; set; }
    
    public UnitAdvancedDefenses AsReadonly()
    {
        return new UnitAdvancedDefenses(Shield, RegenerationPercent);
    }
}

public enum ShieldStatus
{
    NoShield,
    Buckler,
    Shield,
    LargeShield
}

public struct UnitPosition
{
    private readonly int _xPos;
    public int XPos { get { return _xPos; } }

    private readonly int _yPos;
    public int YPos { get { return _yPos; } }

    public UnitPosition(int xPos, int yPos)
    {
        _xPos = xPos;
        _yPos = yPos;
    }
}

public class UnitPositionBuilder
{
    public int XPos { get; set; }
    
    public int YPos { get; set; }

    public UnitPositionBuilder(int xPos, int yPos)
    {
        XPos = xPos;
        YPos = yPos;
    }

    public UnitPosition ToReadonly()
    {
        return new UnitPosition(XPos, YPos);
    }
}

public struct UnitMeteredAttribute
{
    private readonly int _max;
    public int Max { get{ return _max; } }

    private readonly int _current;
    public int Current{ get{ return _current; } }

    public UnitMeteredAttribute(int max, int current)
    {
        _max = max;
        _current = current;
    }
}

public class UnitMeteredAttributeBuilder
{
    public int Max { get; set; }

    public int Current { get; set; }

    public UnitMeteredAttributeBuilder(int max, int current)
    {
        Max = max;
        Current = current;
    }

    public UnitMeteredAttribute ToReadonly()
    {
        return new UnitMeteredAttribute(Max, Current);
    }
}

public struct UnitFightBasics
{
    private readonly int _strength;
    public int Strength { get { return _strength; } }

    private readonly int _precision;
    public int Precision { get { return _precision; } }

    private readonly int _attack;
    public int Attack { get { return _attack; } }

    private readonly int _defense;
    public int Defense { get { return _defense; } }

    public UnitFightBasics(int strength, int precision, int attack, int defense)
    {
        _strength = strength;
        _precision = precision;
        _attack = attack;
        _defense = defense;
    }
}

public class UnitFightBasicsBuilder
{
    public int Strength { get; set; }

    public int Precision { get; set; }

    public int Attack { get; set; }

    public int Defense { get; set; }

    public UnitFightBasicsBuilder(int strength, int precision, int attack, int defense)
    {
        Strength = strength;
        Precision = precision;
        Attack = attack;
        Defense = defense;
    }

    public UnitFightBasics ToReadonly()
    {
        return new UnitFightBasics(Strength, Precision, Attack, Defense);
    }
}

public struct UnitAttributes
{
    private readonly UnitPosition _position;
    public UnitPosition Position { get{ return _position; } }

    private readonly int _size;
    public int Size { get { return _size; } }

    private readonly int _movement;
    public int Movement { get { return _movement; } }

    private readonly UnitMeteredAttribute _hitPoints;
    public UnitMeteredAttribute HitPoints{ get{ return _hitPoints; } }

    private readonly UnitMeteredAttribute _endurance;
    public UnitMeteredAttribute Endurance { get { return _endurance; } }

    private readonly UnitMeteredAttribute _moral;
    public UnitMeteredAttribute Moral { get{ return _moral; } }

    private readonly UnitFightBasics _fightBasics;
    public UnitFightBasics FightBasics{ get{ return _fightBasics; } }

    public UnitAttributes(UnitPosition position,
        int size,
        int movement,
        UnitMeteredAttribute hitPoints,
        UnitMeteredAttribute endurance,
        UnitMeteredAttribute moral,
        UnitFightBasics fightBasics)
    {
        _position = position;
        _size = size;
        _movement = movement;
        _hitPoints = hitPoints;
        _endurance = endurance;
        _moral = moral;
        _fightBasics = fightBasics;
    }
}

public class UnitAttributesBuilder
{
    private readonly UnitPositionBuilder _position;
    public UnitPositionBuilder Position { get { return _position; } }
    
    public int Size { get; set; }
    
    public int Movement { get; set; }

    private readonly UnitMeteredAttributeBuilder _hitPoints;
    public UnitMeteredAttributeBuilder HitPoints { get { return _hitPoints; } }

    private readonly UnitMeteredAttributeBuilder _endurance;
    public UnitMeteredAttributeBuilder Endurance { get { return _endurance; } }

    private readonly UnitMeteredAttributeBuilder _moral;
    public UnitMeteredAttributeBuilder Moral { get { return _moral; } }

    private readonly UnitFightBasicsBuilder _fightBasics;
    public UnitFightBasicsBuilder FightBasics { get { return _fightBasics; } }

    public UnitAttributesBuilder(UnitPositionBuilder position,
        int size,
        int movement,
        UnitMeteredAttributeBuilder hitPoints,
        UnitMeteredAttributeBuilder endurance,
        UnitMeteredAttributeBuilder moral,
        UnitFightBasicsBuilder fightBasics)
    {
        _position = position;
        Size = size;
        Movement = movement;
        _hitPoints = hitPoints;
        _endurance = endurance;
        _moral = moral;
        _fightBasics = fightBasics;
    }


    public UnitAttributes ToReadonly()
    {
        return new UnitAttributes(Position.ToReadonly(),
            Size,
            Movement,
            HitPoints.ToReadonly(),
            Endurance.ToReadonly(),
            Moral.ToReadonly(),
            FightBasics.ToReadonly()
            );
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

public struct RangedAttack
{
    private readonly int _attackPower;
    public int AttackPower { get{ return _attackPower; } }

    private readonly DamageType _damageType;
    public DamageType DamageType { get{ return _damageType; } }

    private readonly int _minimumRange;
    public int MinimumRange { get{ return _minimumRange; } }

    private readonly int _maximumRange;
    public int MaximumRange { get{ return _maximumRange; } }

    private readonly AreaOfEffectType _areaOfEffect;
    public AreaOfEffectType AreaOfEffect { get{ return _areaOfEffect; } }

    private readonly int _ammunition;
    public int Ammunition { get{ return _ammunition; } }

    public RangedAttack(int attackPower,
        DamageType damageType,
        int minimumRange,
        int maximumRange,
        AreaOfEffectType areaOfEffect,
        int ammunition)
    {
        _attackPower = attackPower;
        _damageType = damageType;
        _minimumRange = minimumRange;
        _maximumRange = maximumRange;
        _areaOfEffect = areaOfEffect;
        _ammunition = ammunition;
    }
}

public class RangedAttackBuilder
{
    public int AttackPower { get; set; }
    public DamageType DamageType { get; set; }
    public int MinimumRange { get; set; }
    public int MaximumRange { get; set; }
    public AreaOfEffectType AreaOfEffect { get; set; }
    public int Ammunition { get; set; }

    public RangedAttackBuilder(int attackPower,
        DamageType damageType,
        int minimumRange,
        int maximumRange,
        AreaOfEffectType areaOfEffect,
        int ammunition)
    {
        AttackPower = attackPower;
        DamageType = damageType;
        MinimumRange = minimumRange;
        MaximumRange = maximumRange;
        AreaOfEffect = areaOfEffect;
        Ammunition = ammunition;
    }
    
    public RangedAttack ToReadonly()
    {
        return new RangedAttack(AttackPower,
            DamageType,
            MinimumRange,
            MaximumRange,
            AreaOfEffect,
            Ammunition);
    }
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

public struct MeleeAttack
{
    private readonly int _attackPower;
    public int AttackPower { get{ return _attackPower; } }

    private readonly DamageType _damageType;
    public DamageType DamageType { get{ return _damageType; } }

    private readonly AreaOfEffectType _areaOfAffect;
    public AreaOfEffectType AreaOfEffect { get{ return _areaOfAffect; } }

    public MeleeAttack(int attackPower,
        DamageType damageType,
        AreaOfEffectType areaOfEffect)
    {
        _attackPower = attackPower;
        _damageType = damageType;
        _areaOfAffect = areaOfEffect;
    }
}

public class MeleeAttackBuilder
{
    public int AttackPower { get; set; }
    public DamageType DamageType { get; set; }
    public AreaOfEffectType AreaOfEffect { get; set; }

    public MeleeAttackBuilder(int attackPower,
        DamageType damageType,
        AreaOfEffectType areaOfEffect)
    {
        AttackPower = attackPower;
        DamageType = damageType;
        AreaOfEffect = areaOfEffect;
    }

    public MeleeAttack ToReadonly()
    {
        return new MeleeAttack(AttackPower, DamageType, AreaOfEffect);
    }
}