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

    private readonly UnitEmotions _emotions;
    public UnitEmotions Emotions { get{ return _emotions; } }
    
    private readonly UnitOffense _offense;
    public UnitOffense Offense { get { return _offense; } }

    private readonly UnitDefenses _defense;
    public UnitDefenses Defense { get { return _defense; } }

    public UnitAttributes(UnitPosition position,
        int size,
        int movement,
        UnitMeteredAttribute hitPoints,
        UnitEmotions emotions,
        UnitOffense offense,
        UnitDefenses defenses)
    {
        _position = position;
        _size = size;
        _movement = movement;
        _hitPoints = hitPoints;
        _emotions = emotions;
        _offense = offense;
        _defense = defenses;
    }
}