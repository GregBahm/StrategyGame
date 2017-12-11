using System.Collections.Generic;
using System.Linq;
public class UnitState
{
    private readonly UnitIdentification _identification;
    public UnitIdentification Identification { get { return _identification; } }

    private readonly List<MeleeAttack> _meleeAttacks;
    public List<MeleeAttack> MeleeAttacks { get { return _meleeAttacks; } }

    private readonly List<RangedAttack> _rangedAttacks;
    public List<RangedAttack> RangedAttacks { get { return _rangedAttacks; } }

    private readonly UnitPosition _position;
    public UnitPosition Position { get { return _position; } }

    public int Size { get; set; }

    public int Movement { get; set; }

    private readonly UnitMeteredAttribute _hitPoints;
    public UnitMeteredAttribute HitPoints { get { return _hitPoints; } }

    private readonly UnitEmotions _emotions;
    public UnitEmotions Emotions { get { return _emotions; } }

    private readonly UnitOffense _offense;
    public UnitOffense Offense { get { return _offense; } }

    private readonly UnitDefenses _defense;
    public UnitDefenses Defense { get { return _defense; } }


    public bool IsDefeated { get; set; }

    public UnitState(UnitIdentification description)
    {
        _identification = description;
        _position = new UnitPosition();
        _hitPoints = new UnitMeteredAttribute();
        _emotions = new UnitEmotions();
        _offense = new UnitOffense();
        _defense = new UnitDefenses();
        _meleeAttacks = new List<MeleeAttack>();
        _rangedAttacks = new List<RangedAttack>();
    }

    public UnitStateRecord AsReadonly()
    {
        return new UnitStateRecord(Identification, 
            Position.AsReadonly(),
            Size,
            Movement,
            HitPoints.AsReadonly(),
            Emotions.AsReadonly(),
            Offense.AsReadonly(),
            Defense.AsReadonly(),
            MeleeAttacks.Select(item => item.AsReadonly()).ToArray(),
            RangedAttacks.Select(item => item.AsReadonly()).ToArray(),
            IsDefeated);
    }
}
