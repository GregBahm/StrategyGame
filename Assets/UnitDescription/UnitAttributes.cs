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
