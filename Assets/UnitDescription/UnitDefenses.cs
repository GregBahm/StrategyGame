public class UnitDefenses
{
    public int Armor { get; set; }

    public ShieldStatus Shield { get; set; }

    public int Regeneration { get; set; }
    
    public UnitDefensesRecord AsReadonly()
    {
        return new UnitDefensesRecord(Armor, Shield, Regeneration);
    }
}
