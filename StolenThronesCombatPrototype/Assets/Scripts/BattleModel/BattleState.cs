using System;
using System.Collections.Generic;
using System.Linq;

public class BattleState
{
    public BattleStageSide LeftSide { get; }
    public BattleStageSide RightSide { get; }

    public BattleStatus Status { get; }

    public BattleState(BattleStageSide leftSide, BattleStageSide rightSide)
    {
        LeftSide = leftSide;
        RightSide = rightSide;
        Status = GetStatus();
    }

    private BattleStatus GetStatus()
    {
        if (LeftSide.StillFighting && RightSide.StillFighting) return BattleStatus.Ongoing;
        if (LeftSide.StillFighting && !RightSide.StillFighting) return BattleStatus.LeftWins;
        if (!LeftSide.StillFighting && RightSide.StillFighting) return BattleStatus.RightWins;
        return BattleStatus.Draw;
    }
    
    public IEnumerable<BattalionBattleEffects> GetUnitEffects()
    {
        foreach (BattalionState item in LeftSide)
        {
            yield return item.GetEffects(LeftSide, RightSide);
        }
        foreach (BattalionState item in RightSide)
        {
            yield return item.GetEffects(RightSide, LeftSide);
        }
    }

    internal BattleState GetWithDefeatedRemoved()
    {
        BattleStageSide newLeftSide = LeftSide.GetRepositionedSurvivors();
        BattleStageSide newRightSide = RightSide.GetRepositionedSurvivors();
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
        IEnumerable<BattalionSpawnEffect> spawns = effects.SelectMany(item => item.UnitSpawns).ToList();
        return GetNewBattleState(modifiedUnits, spawns);
    }

    private BattleState GetNewBattleState(IEnumerable<BattalionState> modifiedUnits, IEnumerable<BattalionSpawnEffect> spawns)
    {
        ModifiedSideSorter leftSideSorter = new ModifiedSideSorter(LeftSide);
        ModifiedSideSorter rightSideSorter = new ModifiedSideSorter(RightSide);

        foreach (BattalionState battalionState in modifiedUnits)
        {
            leftSideSorter.Incorporate(battalionState);
            rightSideSorter.Incorporate(battalionState);
        }

        foreach (BattalionSpawnEffect spawn in spawns)
        {
            IncorporateSpawn(spawn, leftSideSorter, rightSideSorter);
        }

        BattleStageSide left = leftSideSorter.ToSide();
        BattleStageSide right = rightSideSorter.ToSide();
        return new BattleState(left, right);
    }

    private void IncorporateSpawn(BattalionSpawnEffect spawn, 
        ModifiedSideSorter leftSideSorter, 
        ModifiedSideSorter rightSideSorter)
    {
        //TODO: Get the spawns spawning here
    }

    private class ModifiedSideSorter
    {
        Dictionary<BattalionIdentifier, BattalionState> unitsDictionary;

        public ModifiedSideSorter(BattleStageSide basis)
        {
            unitsDictionary = basis.ToDictionary(item => item.Id);
        }

        public BattleStageSide ToSide()
        {
            List<BattalionState> units = unitsDictionary.Values.Where(item => item != null).ToList();
            return new BattleStageSide(units);
        }

        internal void Incorporate(BattalionState state)
        {
            if(unitsDictionary.ContainsKey(state.Id))
            {
                unitsDictionary[state.Id] = state;
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
