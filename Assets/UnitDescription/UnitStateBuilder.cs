using System.Collections.Generic;
using System.Linq;
public class UnitStateBuilder
{
    private readonly UnitIdentification _identification;
    public UnitIdentification Identification { get { return _identification; } }

    private readonly UnitAttributesBuilder _attributes;
    public UnitAttributesBuilder Attributes { get { return _attributes; } }

    private readonly List<MeleeAttackBuilder> _meleeAttacks;
    public List<MeleeAttackBuilder> MeleeAttacks { get { return _meleeAttacks; } }

    private readonly List<RangedAttackBuilder> _rangedAttacks;
    public List<RangedAttackBuilder> RangedAttacks { get { return _rangedAttacks; } }

    public bool IsDefeated { get; set; }

    public UnitStateBuilder(UnitIdentification description)
    {
        _identification = description;
        _attributes = new UnitAttributesBuilder();
        _meleeAttacks = new List<MeleeAttackBuilder>();
        _rangedAttacks = new List<RangedAttackBuilder>();
    }

    public UnitState AsReadonly()
    {
        return new UnitState(Identification,
            Attributes.AsReadonly(),
            MeleeAttacks.Select(item => item.AsReadonly()).ToArray(),
            RangedAttacks.Select(item => item.AsReadonly()).ToArray(),
            IsDefeated);
    }
}
