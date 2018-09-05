public class CombatOutcome
{
    public ArmyMove ArmyBeforeCombat { get; }
    public bool Victorious { get; }
    public Army ArmyAfterCombat { get; }

    public CombatOutcome(ArmyMove armyBeforeCombat,
        bool victorious,
        Army armyAfterCombat)
    {
        ArmyBeforeCombat = armyBeforeCombat;
        Victorious = victorious;
        ArmyAfterCombat = armyAfterCombat;
    }
}
