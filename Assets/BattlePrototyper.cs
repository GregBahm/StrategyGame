using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattlePrototyper : MonoBehaviour 
{
    private void Start()
    {
        List<UnitStateBuilder> attackingUnits = GetPrototypeAttackers();
        List<UnitStateBuilder> defendingUnits = GetPrototypeDefenders();
        BattleRound battleRound = GetBattleRound(attackingUnits, defendingUnits);
        DisplayBattleRound(battleRound);
    }

    private void DisplayBattleRound(BattleRound battleRound)
    {
        foreach (UnitState state in battleRound.AttackingUnits.Concat(battleRound.DefendingUnits))
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = new Vector3(state.Attributes.Position.XPos, 0, state.Attributes.Position.YPos);
            obj.name = state.Description.Name;
        }
    }

    private BattleRound GetBattleRound(List<UnitStateBuilder> attackingUnits, List<UnitStateBuilder> defendingUnits)
    {
        UnitState[] attackingState = attackingUnits.Select(item => item.AsReadonly()).ToArray();
        UnitState[] defendingState = defendingUnits.Select(item => item.AsReadonly()).ToArray();
        BattleStatus status = GetBattleStatus(attackingState, defendingState);
        return new BattleRound(attackingState, defendingState, status);
    }

    private BattleStatus GetBattleStatus(UnitState[] attackingState, UnitState[] defendingState)
    {
        if(attackingState.Any())
        {
            if(defendingState.Any())
            {
                return BattleStatus.Ongoing;
            }
            return BattleStatus.AttackersVictorious;
        }
        {
            return BattleStatus.DefendersVictorious;
        }
    }

    private List<UnitStateBuilder> GetPrototypeDefenders()
    {
        List<UnitStateBuilder> ret = new List<UnitStateBuilder>();
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(10, i * 3 + 40));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(5, i * 3 + 10));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetKnight(10, i * 3));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetKnight(10, i * 3 + 100));
        }
        return ret;
    }

    private List<UnitStateBuilder> GetPrototypeAttackers()
    {
        List<UnitStateBuilder> ret = new List<UnitStateBuilder>();
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(90, i * 3 + 40));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(95, i * 3 + 40));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(97, i * 3 + 40));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetTroll(85, i * 3 + 60));
        }
        return ret;
    }
}

public class BattleRound
{
    private readonly IEnumerable<UnitState> _attackingUnits;
    public IEnumerable<UnitState> AttackingUnits { get{ return _attackingUnits; } }

    private readonly IEnumerable<UnitState> _defendingUnits;
    public IEnumerable<UnitState> DefendingUnits { get{ return _defendingUnits; } }

    private readonly BattleStatus _status;
    public BattleStatus Status { get{ return _status; } }
    
    public BattleRound(IEnumerable<UnitState> attackingUnits, IEnumerable<UnitState> defendingUnits, BattleStatus status)
    {
        _attackingUnits = attackingUnits;
        _defendingUnits = defendingUnits;
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

    public UnitStateBuilder(UnitDescription description)
    {
        _description = description;
        _attributes = new UnitAttributesBuilder();
        _meleeAttacks = new List<MeleeAttackBuilder>();
        _rangedAttacks = new List<RangedAttackBuilder>();
    }

    public UnitState AsReadonly()
    {
        return new UnitState(Description,
            Attributes.AsReadonly(),
            MeleeAttacks.Select(item => item.AsReadonly()).ToArray(),
            RangedAttacks.Select(item => item.AsReadonly()).ToArray());
    }
}

public struct UnitDefenses
{
    private readonly int _dodging;
    public int Dodging { get { return _dodging; } }

    private readonly int _armor;
    public int Armor { get { return _armor; } }

    private readonly ShieldStatus _shield;
    public ShieldStatus Shield { get { return _shield; } }

    private readonly int _regenerationPercent;
    public int RegenerationPercent { get{ return _regenerationPercent; } }

    public UnitDefenses(int defense,
        int armor,
        ShieldStatus shield, 
        int regenerationPercent)
    {
        _dodging = defense;
        _armor = armor;
        _shield = shield;
        _regenerationPercent = regenerationPercent;
    }
}

public class UnitDefensesBuilder
{
    public int Dodging { get; set; }

    public int Armor { get; set; }

    public ShieldStatus Shield { get; set; }

    public int RegenerationPercent { get; set; }
    
    public UnitDefenses AsReadonly()
    {
        return new UnitDefenses(Dodging, Armor, Shield, RegenerationPercent);
    }
}

public enum ShieldStatus
{
    None,
    Small,
    Medium,
    Large
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

    public UnitPosition AsReadonly()
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

    public UnitMeteredAttribute AsReadonly()
    {
        return new UnitMeteredAttribute(Max, Current);
    }
}

public class UnitOffense
{
    private readonly int _strength;
    public int Strength { get { return _strength; } }

    private readonly int _precision;
    public int Precision { get { return _precision; } }

    private readonly int _attackAccuracy;
    public int AttackAccuracy { get { return _attackAccuracy; } }

    public UnitOffense(int strength, int precision, int attackAccuracy)
    {
        _strength = strength;
        _precision = precision;
        _attackAccuracy = attackAccuracy;
    }
}

public class UnitOffenseBuilder
{
    public int Strength { get; set; }

    public int Precision { get; set; }

    public int AttackAccuracy { get; set; }

    public UnitOffense AsReadonly()
    {
        return new UnitOffense(Strength, Precision, AttackAccuracy);
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
    
    private readonly UnitOffense _offense;
    public UnitOffense Offense { get { return _offense; } }

    private readonly UnitDefenses _defense;
    public UnitDefenses Defense { get { return _defense; } }

    public UnitAttributes(UnitPosition position,
        int size,
        int movement,
        UnitMeteredAttribute hitPoints,
        UnitMeteredAttribute endurance,
        UnitMeteredAttribute moral,
        UnitOffense offense,
        UnitDefenses defenses)
    {
        _position = position;
        _size = size;
        _movement = movement;
        _hitPoints = hitPoints;
        _endurance = endurance;
        _moral = moral;
        _offense = offense;
        _defense = defenses;
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

    private readonly UnitOffenseBuilder _offense;
    public UnitOffenseBuilder Offense { get { return _offense; } }

    private readonly UnitDefensesBuilder _defense;
    public UnitDefensesBuilder Defense { get { return _defense; } }

    public UnitAttributesBuilder()
    {
        _position = new UnitPositionBuilder();
        _hitPoints = new UnitMeteredAttributeBuilder();
        _endurance = new UnitMeteredAttributeBuilder();
        _moral = new UnitMeteredAttributeBuilder();
        _offense = new UnitOffenseBuilder();
        _defense = new UnitDefensesBuilder();
    }


    public UnitAttributes AsReadonly()
    {
        return new UnitAttributes(Position.AsReadonly(),
            Size,
            Movement,
            HitPoints.AsReadonly(),
            Endurance.AsReadonly(),
            Moral.AsReadonly(),
            Offense.AsReadonly(),
            Defense.AsReadonly()
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
    
    public RangedAttack AsReadonly()
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

    private readonly bool _chargeAttack;
    public bool ChargeAttack { get{ return _chargeAttack; } }

    public MeleeAttack(int attackPower,
        DamageType damageType,
        AreaOfEffectType areaOfEffect,
        bool chargeAttack)
    {
        _attackPower = attackPower;
        _damageType = damageType;
        _areaOfAffect = areaOfEffect;
        _chargeAttack = true;
    }
}

public class MeleeAttackBuilder
{
    public int AttackPower { get; set; }
    public DamageType DamageType { get; set; }
    public AreaOfEffectType AreaOfEffect { get; set; }
    public bool ChargeAttack { get; set; }

    public MeleeAttack AsReadonly()
    {
        return new MeleeAttack(AttackPower, DamageType, AreaOfEffect, ChargeAttack);
    }
}