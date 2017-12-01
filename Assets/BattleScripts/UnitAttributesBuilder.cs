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
