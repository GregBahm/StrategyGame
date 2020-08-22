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

    private readonly Dictionary<BattalionIdentifier, BattleSide> sideLookup;

    public BattleState(BattleStateSide leftSide, BattleStateSide rightSide)
    {
        LeftSide = leftSide;
        RightSide = rightSide;
        Status = GetStatus();
        sideLookup = GetSideLookup();
    }

    private Dictionary<BattalionIdentifier, BattleSide> GetSideLookup()
    {
        Dictionary<BattalionIdentifier, BattleSide> ret = new Dictionary<BattalionIdentifier, BattleSide>();
        foreach (BattalionState item in LeftSide)
        {
            ret.Add(item.Id, BattleSide.Left);
        }
        foreach (BattalionState item in RightSide)
        {
            ret.Add(item.Id, BattleSide.Right);
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

    internal BattleSide GetSide(BattalionIdentifier id)
    {
        return sideLookup[id];
    }

    public IEnumerable<BattalionBattleEffects> GetUnitEffects()
    {
        foreach (var item in LeftSide)
        {
            yield return item.GetEffects(LeftSide, RightSide);
        }
        foreach (var item in RightSide)
        {
            yield return item.GetEffects(RightSide, LeftSide);
        }
    }

    internal BattleState GetWithDefeatedRemoved()
    {
        BattleStateSide newLeftSide = LeftSide.GetWithDefeatedRemoved();
        BattleStateSide newRightSide = RightSide.GetWithDefeatedRemoved();
        return new BattleState(newLeftSide, newRightSide);
    }

    public BattleState GetWithEffectsApplied(IEnumerable<BattalionBattleEffects> effects)
    {
        Dictionary<BattalionIdentifier, EffectsBuilder> dictionary = GetEffectsDictionary();
        foreach (BattalionStateModifier item in effects.SelectMany(item => item.UnitModifications))
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
        readonly Dictionary<BattalionIdentifier, BattalionState> sorter;

        public ModifiedSideSorter(BattleStateSide basis)
        {
            sorter = basis.ToDictionary(item => item.Id, item => item);
        }

        public BattleStateSide ToSide()
        {
            List<BattalionState> newSide = sorter.Values.ToList();
            return new BattleStateSide(newSide);
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

        List<BattalionState> units = LeftSide.ToList();
        units.AddRange(RightSide);
        foreach (BattalionState unit in units)
        {
            ret.Add(unit.Id, new EffectsBuilder(unit));
        }
        return ret;
    }
}
