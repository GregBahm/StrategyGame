using System.Collections.Generic;
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
