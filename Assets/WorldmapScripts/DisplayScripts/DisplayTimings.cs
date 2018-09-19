using System.Linq;
using UnityEngine;

public class DisplayTimings
{
    private static TimingsMaster _timingsMaster = new TimingsMaster();
    private readonly float _turnProgression;

    public float RallyChangesAndNewArmies { get { return _timingsMaster.RallyChangesAndNewArmies.GetSubprogress(_turnProgression); } }
    public float NewUnits { get { return _timingsMaster.NewUnits.GetSubprogress(_turnProgression); } }
    public float UnitsMove { get { return _timingsMaster.UnitsMove.GetSubprogress(_turnProgression); } }
    public float ProvinceEffects { get { return _timingsMaster.ProvinceEffects.GetSubprogress(_turnProgression); } }
    public float RoutingArmyRecovery { get { return _timingsMaster.RoutingArmyRecovery.GetSubprogress(_turnProgression); } }

    public float ArmiesMoveToCollision { get { return _timingsMaster.ArmiesMoveToCollision.GetSubprogress(_turnProgression); } }
    public float ArmiesDieFromCollisionBattles { get { return _timingsMaster.ArmiesDieFromCollisionBattles.GetSubprogress(_turnProgression); } }
    public float ArmiesToDestination { get { return _timingsMaster.ArmiesToDestination.GetSubprogress(_turnProgression); } }
    public float ArmiesDieFromNonCollisionBattles { get { return _timingsMaster.ArmiesDieFromNonCollisionBattles.GetSubprogress(_turnProgression); } }

    public float ProvinceOwnershipChanges { get { return _timingsMaster.ProvinceOwnershipChanges.GetSubprogress(_turnProgression); } }
    public float ProvinceMergers { get { return _timingsMaster.ProvinceMergers.GetSubprogress(_turnProgression); } }
    public float ProvinceUpgrades { get { return _timingsMaster.ProvinceUpgrades.GetSubprogress(_turnProgression); } }

    public float PlayersDead { get { return _timingsMaster.PlayersDead.GetSubprogress(_turnProgression); } }

    public DisplayTimings(float turnProgression)
    {
        _turnProgression = turnProgression;
    }


    private class TimingsMaster
    {
        public Phase NewUnits { get; }
        public Phase ProvinceEffects { get; }
        public Phase RoutingArmyRecovery { get; }
        public Phase ArmiesMoveToCollision { get; }
        public Phase ArmiesDieFromCollisionBattles { get; }
        public Phase ArmiesToDestination { get; }
        public Phase ArmiesDieFromNonCollisionBattles { get; }

        public Phase ProvinceOwnershipChanges { get; }
        public Phase ProvinceMergers { get; }
        public Phase ProvinceUpgrades { get; }

        public Phase RallyChangesAndNewArmies { get; }
        public Phase UnitsMove { get; }
        public Phase PlayersDead { get; }

        public TimingsMaster()
        {
            NewUnits = new Phase(1);
            ProvinceEffects = new Phase(1);
            RoutingArmyRecovery = new Phase(1);
            ArmiesMoveToCollision = new Phase(1);
            ArmiesDieFromCollisionBattles = new Phase(1);
            ArmiesToDestination = new Phase(1);
            ArmiesDieFromNonCollisionBattles = new Phase(1);
            ProvinceOwnershipChanges = new Phase(1);
            ProvinceMergers = new Phase(1);
            ProvinceUpgrades = new Phase(1);
            RallyChangesAndNewArmies = new Phase(1);
            UnitsMove = new Phase(1);
            PlayersDead = new Phase(1);

            Phase[] phaseOrder = new Phase[]
            {
                RallyChangesAndNewArmies,
                NewUnits,
                UnitsMove,
                ProvinceEffects,
                RoutingArmyRecovery,
                ArmiesMoveToCollision,
                ArmiesDieFromCollisionBattles,
                ArmiesToDestination,
                ArmiesDieFromNonCollisionBattles,
                ProvinceOwnershipChanges,
                ProvinceMergers,
                ProvinceUpgrades,
                PlayersDead
            };
            SetPhaseTimings(phaseOrder);
        }

        private void SetPhaseTimings(Phase[] phaseOrder)
        {
            float totalTime = phaseOrder.Sum(item => item.RelativeLength);
            float accumulatedTime = 0;
            foreach (Phase phase in phaseOrder)
            {
                phase.Start = accumulatedTime / totalTime;
                accumulatedTime += phase.RelativeLength;
                phase.End = accumulatedTime / totalTime; 
            }
        }
    }

    private class Phase
    {
        public float Start { get; set; }
        public float End { get; set; }
        public float RelativeLength { get; }

        public Phase(float relativeLength)
        {
            RelativeLength = relativeLength;
        }
        public float GetSubprogress(float masterProgress)
        {
            float ret = (masterProgress - Start) / (End - Start);
            ret = Mathf.Clamp01(ret);
            return ret;
        }
    }
}
