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
        foreach (BattalionState item in LeftSide.SelectMany(item => item))
        {
            ret.Add(item.Id, BattleSideIdentifier.Left);
        }
        foreach (BattalionState item in RightSide.SelectMany(item => item))
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
        foreach (BattalionState item in LeftSide.AllUnits)
        {
            IEnumerable<BattalionStateModifier> ret = item.GetEffects(LeftSide, RightSide);
            foreach (var effect in ret)
            {
                yield return effect;
            }
        }
        foreach (BattalionState item in RightSide.AllUnits)
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
        readonly Dictionary<BattalionIdentifier, BattalionState> sorter;

        public ModifiedSideSorter(BattleStateSide basis)
        {
            this.basis = basis;
            sorter = basis.SelectMany(item => item).ToDictionary(item => item.Id, item => item);
        }

        public BattleStateSide ToSide()
        {
            List<BattalionState>[] rankBuilders = GetRankBuilders();
            List<BattalionState> newStates = sorter.Values.ToList();
            foreach (var item in newStates)
            {
                int pos = basis.GetPosition(item.Id);
                rankBuilders[pos].Add(item);
            }
            List<BattleRank> ranks = rankBuilders.Select(item => new BattleRank(item)).ToList();
            return new BattleStateSide(ranks);
        }

        private List<BattalionState>[] GetRankBuilders()
        {
            List<BattalionState>[] ret = new List<BattalionState>[basis.Count];
            for (int i = 0; i < basis.Count; i++)
            {
                ret[i] = new List<BattalionState>();
            }
            return ret;
        }

        internal void Incorporate(BattalionState state)
        {
            if(sorter.ContainsKey(state.Id))
            {
                sorter[state.Id] = state;
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

        List<BattalionState> units = LeftSide.AllUnits.ToList();
        units.AddRange(RightSide.AllUnits);
        foreach (BattalionState unit in units)
        {
            ret.Add(unit.Id, new EffectsBuilder(unit));
        }
        return ret;
    }
}
