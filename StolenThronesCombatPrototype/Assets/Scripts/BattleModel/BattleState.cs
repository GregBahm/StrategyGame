using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;

public class BattleState
{
    public BattleStateSide LeftSide { get; }
    
    public BattleStateSide RightSide { get; }
    
    public BattleStatus Status { get; }

    private readonly Dictionary<BattalionIdentifier, BattleSideIdentifier> sideLookup;

    public BattleState(BattleStateSide leftSide, BattleStateSide rightSide)
    {
        LeftSide = leftSide;
        RightSide = rightSide;
        Status = GetStatus();
        sideLookup = GetSideLookup();
    }

    private Dictionary<BattalionIdentifier, BattleSideIdentifier> GetSideLookup()
    {
        Dictionary<BattalionIdentifier, BattleSideIdentifier> ret = new Dictionary<BattalionIdentifier, BattleSideIdentifier>();
        foreach (BattalionBattleState item in LeftSide.Units)
        {
            ret.Add(item.Id, BattleSideIdentifier.Left);
        }
        foreach (BattalionBattleState item in RightSide.Units)
        {
            ret.Add(item.Id, BattleSideIdentifier.Right);
        }
        return ret;
    }

    private BattleStatus GetStatus()
    {
        if (LeftSide.StillFighting && RightSide.StillFighting) return BattleStatus.Ongoing;
        if (LeftSide.StillFighting && !RightSide.StillFighting) return BattleStatus.LeftWins;
        if (!LeftSide.StillFighting && RightSide.StillFighting) return BattleStatus.RightWins;
        return BattleStatus.Draw;
    }

    internal BattleSideIdentifier GetSide(BattalionIdentifier id)
    {
        return sideLookup[id];
    }

    public IEnumerable<BattalionStateModifier> GetUnitEffects()
    {
        foreach (BattalionBattleState item in LeftSide.Units)
        {
            IEnumerable<BattalionStateModifier> ret = item.GetEffects(LeftSide, RightSide);
            foreach (var effect in ret)
            {
                yield return effect;
            }
        }
        foreach (BattalionBattleState item in RightSide.Units)
        {
            IEnumerable<BattalionStateModifier> ret = item.GetEffects(RightSide, LeftSide);
            foreach (var effect in ret)
            {
                yield return effect;
            }
        }
    }

    internal BattleState GetWithDefeatedRemoved()
    {
        BattleStateSide newLeftSide = LeftSide.GetWithDefeatedRemoved();
        BattleStateSide newRightSide = RightSide.GetWithDefeatedRemoved();
        return new BattleState(newLeftSide, newRightSide);
    }

    public BattleState GetWithEffectsApplied(IEnumerable<BattalionStateModifier> effects)
    {
        Dictionary<BattalionIdentifier, EffectsBuilder> dictionary = GetEffectsDictionary();
        foreach (BattalionStateModifier item in effects)
        {
            dictionary[item.Target].Add(item);
        }
        List<BattalionState> modifiedUnits = dictionary.Values.Select(item => item.GetNewState()).ToList();
        return GetNewBattleState(modifiedUnits);
    }

    private BattleState GetNewBattleState(IEnumerable<BattalionState> modifiedUnits)
    {
        ModifiedSideSorter leftSideSorter = new ModifiedSideSorter(LeftSide);
        ModifiedSideSorter rightSideSorter = new ModifiedSideSorter(RightSide);

        foreach (BattalionState battalionState in modifiedUnits)
        {
            leftSideSorter.Incorporate(battalionState);
            rightSideSorter.Incorporate(battalionState);
        }

        BattleStateSide left = leftSideSorter.ToSide();
        BattleStateSide right = rightSideSorter.ToSide();
        return new BattleState(left, right);
    }

    private class ModifiedSideSorter
    {
        private readonly BattleStateSide basis;
        readonly Dictionary<BattalionIdentifier, SorterItem> sorter;

        public ModifiedSideSorter(BattleStateSide basis)
        {
            this.basis = basis;
            sorter = basis.Units.ToDictionary(item => item.Id, item => new SorterItem(item.Position));
        }

        public BattleStateSide ToSide()
        {
            IOrderedEnumerable<IGrouping<int, SorterItem>> ordered = sorter.Values.GroupBy(item => item.Position).OrderBy(item => item.Key);
            List<List<BattalionState>> lists =ordered.Select(item => item.Select(sorter => sorter.NewState).ToList()).ToList();
            return new BattleStateSide(lists);
        }

        internal void Incorporate(BattalionState battalionState)
        {
            if(sorter.ContainsKey(battalionState.Id))
            {
                sorter[battalionState.Id].NewState = battalionState;
            }
        }

        private class SorterItem
        {
            public int Position { get; }
            public BattalionState NewState { get; set; }

            public SorterItem(int position)
            {
                Position = position;
            }
        }
    }

    private class EffectsBuilder
    {
        private readonly BattalionState initialState;
        private readonly List<BattalionStateModifier> modifiers = new List<BattalionStateModifier>();

        public EffectsBuilder(BattalionState initialState)
        {
            this.initialState = initialState;
        }

        public void Add(BattalionStateModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public BattalionState GetNewState()
        {
            return initialState.GetWithModifiersApplied(modifiers);
        }
    }

    private Dictionary<BattalionIdentifier, EffectsBuilder> GetEffectsDictionary()
    {
        Dictionary<BattalionIdentifier, EffectsBuilder> ret = new Dictionary<BattalionIdentifier, EffectsBuilder>();

        List<BattalionBattleState> units = LeftSide.Units.ToList();
        units.AddRange(RightSide.Units);
        foreach (BattalionBattleState unit in units)
        {
            ret.Add(unit.Id, new EffectsBuilder(unit));
        }
        return ret;
    }
}
