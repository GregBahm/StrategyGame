public class CombatOutcome
{
    public ArmyMove ArmyBeforeCombat { get; }
    public bool Victorious { get; }
    public ArmyState ArmyAfterCombat { get; }

    public CombatOutcome(ArmyMove armyBeforeCombat,
        bool victorious,
        ArmyState armyAfterCombat)
    {
        ArmyBeforeCombat = armyBeforeCombat;
        Victorious = victorious;
        ArmyAfterCombat = armyAfterCombat;
    }
}
