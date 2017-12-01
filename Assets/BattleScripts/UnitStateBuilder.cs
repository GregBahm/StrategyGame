using System.Collections.Generic;
using System.Linq;
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
