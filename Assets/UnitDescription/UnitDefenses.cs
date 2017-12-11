public class UnitDefenses
{
    public int Dodging { get; set; }

    public int Armor { get; set; }

    public ShieldStatus Shield { get; set; }

    public int RegenerationPercent { get; set; }
    
    public UnitDefensesRecord AsReadonly()
    {
        return new UnitDefensesRecord(Dodging, Armor, Shield, RegenerationPercent);
    }
}
