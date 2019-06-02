using System;
using System.Linq;

public class BattlePreparation
{
    public BattleSidePreparation Attacker { get; }
    public BattleSidePreparation Defender { get; }

    public BattlePreparation(FactionArmies attacker, FactionArmies defender, BattleSite site)
    {
        ScoutingEffects attackerScout = new ScoutingEffects(attacker, defender);
        ScoutingEffects defenderScout = new ScoutingEffects(defender, attacker);
        SpyingEffects attackerSpies = new SpyingEffects(attacker);
        SpyingEffects defenderSpies = new SpyingEffects(defender);
        RaidingEffects raiding = new RaidingEffects(attacker, site);
        LogisticalEffects attackerLogistics = new LogisticalEffects(attacker);
        LogisticalEffects defenderLogistics = new LogisticalEffects(defender);

        Attacker = new BattleSidePreparation(attackerScout, raiding, attackerLogistics, attackerSpies);
        Defender = new BattleSidePreparation(defenderScout, RaidingEffects.Defender, defenderLogistics, defenderSpies);
    }
}


public class BattleSidePreparation
{
    public ScoutingEffects Scouting { get; }
    public RaidingEffects Raiding { get; }
    public LogisticalEffects Logistic { get; }
    public SpyingEffects Spying { get; }

    public BattleSidePreparation(ScoutingEffects scouting, RaidingEffects raiding, LogisticalEffects logistic, SpyingEffects spying)
    {
        Scouting = scouting;
        Raiding = raiding;
        Logistic = logistic;
        Spying = spying;
    }
}

public class ScoutingEffects
{
    public int ScoutBonus { get; }
    public int ScoutsSum { get; }
    public int Scouted { get; }

    public ScoutingEffects(FactionArmies self, FactionArmies other)
    {
        ScoutsSum = self.Armies.Sum(item => item.Scouts.ScoutingEffectiveness);
        Scouted = other.Armies.SelectMany(item => item.Squadrons).Sum(item => GetScoutingVisibility(item));
        ScoutBonus = ScoutsSum * Scouted;
    }

    private int GetScoutingVisibility(Squad item)
    {
        return item.TroopCount * item.Stealth;
    }
}

public class RaidingEffects
{
    public static RaidingEffects Defender { get; } = new RaidingEffects();

    public int RaidingSum { get; }
    public int LogisticsDrain { get; }

    private RaidingEffects()
    { }
    public RaidingEffects(FactionArmies attacker, BattleSite site)
    {
        RaidingSum = attacker.Armies.SelectMany(item => item.Squadrons).Sum(item => item.Raiding);
        LogisticsDrain = Math.Max(0, RaidingSum - site.InitialDefense);
    }
}

public class LogisticalEffects
{
    public int FoodNeeds { get; }
    public int SumFood { get; }
    public int SumMedical { get; }
    public int SumEquipment { get; }

    public LogisticalEffects(FactionArmies forces)
    {
        FoodNeeds = forces.Armies.SelectMany(item => item.Squadrons).Sum(item => item.FoodCost);
        SumFood = forces.Armies.Sum(item => item.Logistics.Food);
        SumMedical = forces.Armies.Sum(item => item.Logistics.Medical);
        SumEquipment = forces.Armies.Sum(item => item.Logistics.Equipment);
    }
}

public class SpyingEffects
{
    public int SpySum { get; }
    public int Sabotage { get; }

    public SpyingEffects(FactionArmies forces)
    {
        SpySum = forces.Armies.Sum(item => item.Spies.LeaderDrain);
        Sabotage = forces.Armies.Sum(item => item.Spies.SupplySabotage);
    }
}

public class Spies
{
    public SpiesDisplayHooks DisplayHooks { get; }
    public int LeaderDrain { get; }
    public int SupplySabotage { get; }

    public Spies(SpiesDisplayHooks displayHooks, int leaderDrain, int supplySabotage)
    {
        DisplayHooks = displayHooks;
        LeaderDrain = leaderDrain;
        SupplySabotage = supplySabotage;
    }
}

public class Logistics
{
    public LogisticsDisplayHooks DisplayHooks { get; }
    public int Food { get; }
    public int Equipment { get; }
    public int Medical { get; }

    public Logistics(LogisticsDisplayHooks displayHooks, int food, int equipment, int medical)
    {
        DisplayHooks = displayHooks;
        Food = food;
        Equipment = equipment;
        Medical = medical;
    }
}

public class Scouts
{
    public ScoutDisplayHooks DisplayHooks { get; }
    public int ScoutingEffectiveness { get; }

    public Scouts(ScoutDisplayHooks displayHooks, int scoutingEffectiveness)
    {
        DisplayHooks = displayHooks;
        ScoutingEffectiveness = scoutingEffectiveness;
    }
}