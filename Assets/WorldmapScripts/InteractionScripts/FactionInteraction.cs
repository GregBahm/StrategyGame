using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FactionInteraction
{
    public Faction Faction { get; }
    public bool Submitted { get; private set; }
    public int RemainingMoves { get { return 3 - (_attacks.Count + _mergers.Count + _upgrades.Count); } }

    public IEnumerable<IIndicatableMove> IndicatableMoves
    {
        get
        {
            foreach (IIndicatableMove item in _attacks)
            {
                yield return item;
            }
            foreach (IIndicatableMove item in _mergers)
            {
                yield return item;
            }
        }
    }

    private readonly FactionsInteractionManager _manager;
    public const int MaxMoves = 3;
    private readonly List<AttackIntention> _attacks;
    private readonly List<MergerIntention> _mergers;
    private readonly List<UpgradeIntention> _upgrades;

    public FactionInteraction(FactionsInteractionManager manager, Faction faction)
    {
        _manager = manager;
        Faction = faction;
        _attacks = new List<AttackIntention>();
        _mergers = new List<MergerIntention>();
        _upgrades = new List<UpgradeIntention>();
    }

    internal void Renew()
    {
        _attacks.Clear();
        _mergers.Clear();
        _upgrades.Clear();
    }
    
    internal void RequestAttackOrMerge(ProvinceState selectedProvince, ProvinceState draggedOnProvince)
    {
        ClearExistingMove(selectedProvince);
        if(RemainingMoves > 0)
        {
            if (selectedProvince.Owner == draggedOnProvince.Owner)
            {
                // Merge
                MergerIntention merger = new MergerIntention(selectedProvince.Owner, selectedProvince.Identifier, draggedOnProvince.Identifier);
                _mergers.Add(merger);
            }
            else
            {
                // Attack
                AttackIntention attack = new AttackIntention(selectedProvince.Owner, selectedProvince.Identifier, draggedOnProvince.Identifier);
                _attacks.Add(attack);
            }
        }
    }

    private void ClearExistingMove(ProvinceState selectedProvince)
    {
        MergerIntention existingMerge = _mergers.FirstOrDefault(item => item.From == selectedProvince.Identifier);
        if(existingMerge != null)
        {
            _mergers.Remove(existingMerge);
        }
        AttackIntention existingAttack = _attacks.FirstOrDefault(item => item.From == selectedProvince.Identifier);
        if(existingAttack != null)
        {
            _attacks.Remove(existingAttack);
        }
    }

    internal IEnumerable<PlayerMove> GetMoves()
    {
        foreach (AttackIntention attack in _attacks)
        {
            yield return attack;
        }
        foreach (MergerIntention merge in _mergers)
        {
            yield return merge;
        }
        foreach (UpgradeIntention upgrade in _upgrades)
        {
            yield return upgrade.ToMove();
        }
    }

    private class AttackIntention : AttackMove, IIndicatableMove
    {
        private readonly FactionInteraction _source;

        public Province From { get; set; }
        public Province To { get; set; }
        
        public AttackIntention(Faction faction, Province from, Province to)
            : base(faction, from, to)
        {
            From = from;
            To = to;
        }
    }

    private class UpgradeIntention
    {
        //Player clicks on province they control.
        //Upgrades display along right side of the screen.
        //Player clicks on an upgrade to set it.

        //With province selected, they can click on the upgrade again to remove it, or click on a different upgrade to replace it

        private readonly FactionInteraction _source;

        public ProvinceDisplay SourceProvince { get; }

        public ProvinceUpgradeBlueprint SelectedUpgrade { get; set; }
        public Tile TargetTile { get; }

        public UpgradeIntention(FactionInteraction source)
        {
            _source = source;
        }

        internal UpgradeMove ToMove()
        {
            ProvinceUpgrade upgrade = new ProvinceUpgrade(SelectedUpgrade, TargetTile);
            return new UpgradeMove(_source.Faction, SourceProvince.Identifier, upgrade);
        }
    }

    private class MergerIntention : MergerMove, IIndicatableMove
    {
        public Province From { get; }
        public Province To { get; }
        
        public MergerIntention(Faction faction, Province from, Province to)
            :base(faction, from, to)
        {
            From = from;
            To = to;
        }
    }
}