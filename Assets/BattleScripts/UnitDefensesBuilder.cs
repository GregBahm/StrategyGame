public class UnitDefensesBuilder
{
    public int Dodging { get; set; }

    public int Armor { get; set; }

    public ShieldStatus Shield { get; set; }

    public int RegenerationPercent { get; set; }
    
    public UnitDefenses AsReadonly()
    {
        return new UnitDefenses(Dodging, Armor, Shield, RegenerationPercent);
    }
}
