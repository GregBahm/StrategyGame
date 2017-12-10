public class UnitAttributesBuilder
{
    private readonly UnitPositionBuilder _position;
    public UnitPositionBuilder Position { get { return _position; } }
    
    public int Size { get; set; }
    
    public int Movement { get; set; }

    private readonly UnitMeteredAttributeBuilder _hitPoints;
    public UnitMeteredAttributeBuilder HitPoints { get { return _hitPoints; } }

    private readonly UnitEmotionsBuilder _emotions;
    public UnitEmotionsBuilder Emotions { get{ return _emotions; } }

    private readonly UnitOffenseBuilder _offense;
    public UnitOffenseBuilder Offense { get { return _offense; } }

    private readonly UnitDefensesBuilder _defense;
    public UnitDefensesBuilder Defense { get { return _defense; } }

    public UnitAttributesBuilder()
    {
        _position = new UnitPositionBuilder();
        _hitPoints = new UnitMeteredAttributeBuilder();
        _emotions = new UnitEmotionsBuilder();
        _offense = new UnitOffenseBuilder();
        _defense = new UnitDefensesBuilder();
    }


    public UnitAttributes AsReadonly()
    {
        return new UnitAttributes(Position.AsReadonly(),
            Size,
            Movement,
            HitPoints.AsReadonly(),
            Emotions.AsReadonly(),
            Offense.AsReadonly(),
            Defense.AsReadonly()
            );
    }
}
public struct UnitEmotions
{
    private readonly UnitMeteredAttribute _endurance;
    public UnitMeteredAttribute Endurance { get { return _endurance; } }

    private readonly UnitMeteredAttribute _moral;
    public UnitMeteredAttribute Moral { get { return _moral; } }

    private readonly bool _isRouting;
    public bool IsRouting{ get{ return _isRouting; } }

    public UnitEmotions(UnitMeteredAttribute endurance,
        UnitMeteredAttribute moral,
        bool isRouting)
    {
        _endurance = endurance;
        _moral = moral;
        _isRouting = isRouting;
    }
}

public class UnitEmotionsBuilder
{
    private readonly UnitMeteredAttributeBuilder _endurance;
    public UnitMeteredAttributeBuilder Endurance { get { return _endurance; } }

    private readonly UnitMeteredAttributeBuilder _moral;
    public UnitMeteredAttributeBuilder Moral { get { return _moral; } }

    public bool IsRouting { get; set; }

    public UnitEmotionsBuilder()
    {
        _endurance = new UnitMeteredAttributeBuilder();
        _moral = new UnitMeteredAttributeBuilder();
    }

    public UnitEmotions AsReadonly()
    {
        return new UnitEmotions(Endurance.AsReadonly(),
            Moral.AsReadonly(),
            IsRouting);
    }
}