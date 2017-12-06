using System.Collections.Generic;
public class UnitState
{
    private readonly UnitIdentification _identification;
    public UnitIdentification Identification { get { return _identification; } }

    private readonly UnitAttributes _attributes;
    public UnitAttributes Attributes { get{ return _attributes; } }

    private readonly IEnumerable<MeleeAttack> _meleeAttacks;
    public IEnumerable<MeleeAttack> MeleeAttacks { get { return _meleeAttacks; } }

    private readonly IEnumerable<RangedAttack> _rangedAttacks;
    public IEnumerable<RangedAttack> RangedAttacks { get { return _rangedAttacks; } }

    private bool _isDefeated;
    public bool IsDefeated { get { return _isDefeated; } }

    public UnitState(UnitIdentification identification, 
        UnitAttributes attributes,
        IEnumerable<MeleeAttack> meleAttacks, 
        IEnumerable<RangedAttack> rangedAttacks,
        bool isDefeated)
    {
        _identification = identification;
        _attributes = attributes;
        _meleeAttacks = meleAttacks;
        _rangedAttacks = rangedAttacks;
        _isDefeated = isDefeated;
    }
}
