using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionInteraction
{
    private readonly FactionsInteractionManager _manager;

    public Faction Faction { get; }

    public bool Submitted { get; private set; }

    private const int MaxMoves = 3;
    
    public int RemainingMoves { get { return 3 - (_armyMoves.Count + _provinceMerges.Count + _provinceUpgrades.Count); } }

    private readonly List<ArmyMoveBuilder> _armyMoves;
    private readonly List<ProvinceMergeBuilder> _provinceMerges;
    private readonly List<ProvinceUpgradeBuilder> _provinceUpgrades;

    public FactionInteraction(FactionsInteractionManager manager, Faction faction)
    {
        _manager = manager;
        Faction = faction;
        _armyMoves = new List<ArmyMoveBuilder>();
        _provinceMerges = new List<ProvinceMergeBuilder>();
        _provinceUpgrades = new List<ProvinceUpgradeBuilder>();
    }

    public void SubmitMove()
    {
        Submitted = true;
        _manager.OnMoveSubmitted();
    }

    internal void Renew()
    {
        _armyMoves.Clear();
        _provinceMerges.Clear();
        _provinceUpgrades.Clear();
    }

    internal IEnumerable<PlayerMove> GetMoves()
    {
        foreach (ArmyMoveBuilder armyMove in _armyMoves.Where(item => item.IsValid))
        {
            yield return armyMove.ToMove();
        }
        foreach (ProvinceMergeBuilder merge in _provinceMerges.Where(item => item.IsValid))
        {
            yield return merge.ToMove();
        }
        foreach (ProvinceUpgradeBuilder upgrade in _provinceUpgrades.Where(item => item.IsValid))
        {
            yield return upgrade.ToMove();
        }
    }

    private class ArmyMoveBuilder
    {
        //Player clicks on an army they control.
        //Provinces that are available to move to are highlit.
        //The army's home province is also highlit in a different way.

        //Player can right click on a neighboring province to define move.

        //With the army selected, they can right click on the province again to remove the move, or right click on the army's home province, 
        //    or right click on a different province to replace the move

        private readonly FactionInteraction _source;

        public ArmyDisplay Army { get; }

        public ProvinceDisplay Target { get; private set; }

        public bool IsValid
        {
            get
            {
                return Target != null;
            }
        }

        public ArmyMoveBuilder(FactionInteraction source, ArmyDisplay army)
        {
            _source = source;
            Army = army;
        }

        internal ArmyMove ToMove()
        {
            if(!IsValid)
            {
                throw new InvalidOperationException("Can't convert Invalid ArmyMoveBuilder to ArmyMove.");
            }
            return new ArmyMove(_source.Faction, Army.Identifier, Target.Identifier);
        }
    }

    private class ProvinceUpgradeBuilder
    {
        //Player clicks on province they control.
        //Upgrades display along right side of the screen.
        //Player clicks on an upgrade to set it.

        //With province selected, they can click on the upgrade again to remove it, or click on a different upgrade to replace it

        private readonly FactionInteraction _source;

        public ProvinceDisplay SourceProvince { get; }

        public ProvinceUpgradeBlueprint SelectedUpgrade { get; set; }
        public TileDisplay TargetTile { get; set; }

        public bool IsValid
        {
            get
            {
                return SelectedUpgrade != null;
            }
        }

        public ProvinceUpgradeBuilder(FactionInteraction source)
        {
            _source = source;
        }

        internal UpgradeMove ToMove()
        {
            if(!IsValid)
            {
                throw new InvalidOperationException("Can't convert Invalid Province Upgrade Builder to Province Upgrade Move");
            }
            ProvinceUpgrade upgrade = new ProvinceUpgrade(SelectedUpgrade, TargetTile, 0);
            return new UpgradeMove(_source.Faction, SourceProvince.Identifier, upgrade);
        }
    }

    private class ProvinceMergeBuilder
    {
        //Player clicks on a province they control.
        //Provinces that can be merged are available.
        //Player right clicks on mergeable tile to complete merge.

        //With province selected, they can right click on province again to remove merge, or right click on neighboring title to replace merge

        private readonly FactionInteraction _source;

        public ProvinceDisplay GrowingProvince { get; }
        public ProvinceDisplay AbsorbedProvince { get; private set; }

        public bool IsValid
        {
            get
            {
                return AbsorbedProvince != null;
            }
        }

        public ProvinceMergeBuilder(FactionInteraction source, ProvinceDisplay growingProvince)
        {
            _source = source;
            GrowingProvince = growingProvince;
        }

        internal MergerMove ToMove()
        {
            if(!IsValid)
            {
                throw new InvalidOperationException("Can't convert invalid ProvinceMergeBuilder to MergeMove");
            }
            return new MergerMove(_source.Faction, GrowingProvince.Identifier, AbsorbedProvince.Identifier);
        }
    }
}