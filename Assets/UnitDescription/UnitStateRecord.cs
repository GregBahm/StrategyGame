using System.Collections.Generic;

public class UnitStateRecord
{
    private readonly UnitIdentification _identification;
    public UnitIdentification Identification { get { return _identification; } }

    private readonly IEnumerable<MeleeAttackRecord> _meleeAttacks;
    public IEnumerable<MeleeAttackRecord> MeleeAttacks { get { return _meleeAttacks; } }

    private readonly IEnumerable<RangedAttackRecord> _rangedAttacks;
    public IEnumerable<RangedAttackRecord> RangedAttacks { get { return _rangedAttacks; } }

    private readonly UnitLocation _position;
    public UnitLocation Position { get { return _position; } }

    private readonly int _size;
    public int Size { get { return _size; } }

    private readonly int _movement;
    public int Movement { get { return _movement; } }

    private readonly UnitMeteredAttributeRecord _hitPoints;
    public UnitMeteredAttributeRecord HitPoints { get { return _hitPoints; } }

    private readonly UnitEmotionsRecord _emotions;
    public UnitEmotionsRecord Emotions { get { return _emotions; } }

    private readonly UnitOffenseRecord _offense;
    public UnitOffenseRecord Offense { get { return _offense; } }

    private readonly UnitDefensesRecord _defense;
    public UnitDefensesRecord Defense { get { return _defense; } }

    private bool _isDefeated;
    public bool IsDefeated { get { return _isDefeated; } }

    private UnitAllegiance _allegiance;
    public UnitAllegiance Allegiance { get{ return _allegiance; } }

    public UnitStateRecord(UnitIdentification identification, 
    UnitLocation position,
        int size,
        int movement,
        UnitMeteredAttributeRecord hitPoints,
        UnitEmotionsRecord emotions,
        UnitOffenseRecord offense,
        UnitDefensesRecord defenses,
        IEnumerable<MeleeAttackRecord> meleAttacks, 
        IEnumerable<RangedAttackRecord> rangedAttacks,
        UnitAllegiance allegiance,
        bool isDefeated)
    {
        _identification = identification;
        _position = position;
        _size = size;
        _movement = movement;
        _hitPoints = hitPoints;
        _emotions = emotions;
        _offense = offense;
        _defense = defenses;
        _meleeAttacks = meleAttacks;
        _rangedAttacks = rangedAttacks;
        _allegiance = allegiance;
        _isDefeated = isDefeated;
    }
}
