using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainScript : MonoBehaviour
{

}

public class Battle
{
    public IReadOnlyCollection<BattleRound> Progression { get; }

    public Battle(BattleStageSide left, BattleStageSide right)
    {
        Progression = GetProgression(left, right).AsReadOnly();
    }

    private List<BattleRound> GetProgression(BattleStageSide left, BattleStageSide right)
    {
        BattleState currentState = new BattleState(left, right);

        List<BattleRound> ret = new List<BattleRound>();
        while (currentState.Status == BattleStatus.Ongoing)
        {
            IEnumerable<UnitBattleEffects> effects = currentState.GetUnitEffects().ToArray();
            BattleState finalState = currentState.GetWithEffectsApplied(effects);
            BattleRound round = new BattleRound(currentState, effects, finalState);
            ret.Add(round);

            currentState = finalState;
        }
        return ret;
    }
}

public class BattleRound
{
    public BattleState InitialState { get; }
    public IEnumerable<UnitBattleEffects> Effects { get; }
    public BattleState FinalState { get; }
    public BattleRound(BattleState initialState, 
        IEnumerable<UnitBattleEffects> effects, 
        BattleState finalState)
    {
        InitialState = initialState;
        Effects = effects;
        FinalState = finalState;
    }
}

public enum BattleStatus
{
    Ongoing,
    LeftWins,
    RightWins,
    Draw
}

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
    
    public IEnumerable<UnitBattleEffects> GetUnitEffects()
    {
        foreach (var item in LeftSide.Units)
        {
            yield return item.GetUnitEffects(this);
        }
        foreach (var item in RightSide.Units)
        {
            yield return item.GetUnitEffects(this);
        }
    }

    public BattleState GetWithEffectsApplied(IEnumerable<UnitBattleEffects> effects)
    {
        Dictionary<BattleUnit, EffectsBuilder> dictionary = GetEffectsDictionary();
        foreach (UnitStateModifier item in effects.SelectMany(item => item.UnitModifications))
        {
            dictionary[item.Target].Add(item);
        }
        List<BattleUnitState> newUnits = dictionary.Values.Select(item => item.GetNewState()).ToList();
        newUnits.AddRange(effects.SelectMany(item => item.UnitSpawns));
        return GetNewBattleState(newUnits);
    }

    private BattleState GetNewBattleState(List<BattleUnitState> newUnits)
    {
        List<BattleUnitState> newLeftUnits = new List<BattleUnitState>();
        List<BattleUnitState> newRightUnits = new List<BattleUnitState>();
        foreach (BattleUnitState unit in newUnits)
        {
            if (unit.Side == Side.Left)
                newLeftUnits.Add(unit);
            else
                newRightUnits.Add(unit);
        }
        BattleStageSide newLeft = new BattleStageSide(newLeftUnits);
        BattleStageSide newRight = new BattleStageSide(newRightUnits);
        return new BattleState(newLeft, newRight);
    }

    private class EffectsBuilder
    {
        private readonly BattleUnitState initialState;
        private readonly List<UnitStateModifier> modifiers = new List<UnitStateModifier>();

        public EffectsBuilder(BattleUnitState initialState)
        {
            this.initialState = initialState;
        }

        public void Add(UnitStateModifier modifier)
        {
            modifiers.Add(modifier);
        }

        public BattleUnitState GetNewState()
        {
            return initialState.GetWithModifiersApplied(modifiers);
        }
    }

    private Dictionary<BattleUnit, EffectsBuilder> GetEffectsDictionary()
    {
        Dictionary<BattleUnit, EffectsBuilder> ret = new Dictionary<BattleUnit, EffectsBuilder>();

        List<BattleUnitState> units = LeftSide.Units.ToList();
        units.AddRange(RightSide.Units);
        foreach (BattleUnitState unit in units)
        {
            ret.Add(unit.Id, new EffectsBuilder(unit));
        }
        return ret;
    }
}

public class BattleStageSide
{
    public IEnumerable<BattleUnitState> Units { get; }
    
    public bool StillFighting { get; }

    public BattleStageSide(IEnumerable<BattleUnitState> units)
    {
        Units = units;
        StillFighting = GetIsStillFighting();
    }

    private bool GetIsStillFighting()
    {
        return Units.Any(unit => unit.IsAlive);
    }
}

public class BattleUnit
{

}

public class BattleUnitState
{
    public BattleUnit Id { get; }

    public Side Side { get; }

    public float RemainingHitpoints { get; }

    public bool IsAlive { get { return RemainingHitpoints > 0 && !Positiong.EscapedBattlefield; } }
    
    public IEnumerable<BattleUnitAttack> EffectSources { get; }

    public PositioningState Positiong { get; }
    
    public BattleUnitState GetWithModifiersApplied(IEnumerable<UnitStateModifier> modifiers)
    {

    }

    public UnitBattleEffects GetUnitEffects(BattleState state)
    {
        List<UnitStateModifier> modifiers = new List<UnitStateModifier>();
        List<BattleUnitState> spawns = new List<BattleUnitState>();
        
        foreach (BattleUnitAttack attack in EffectSources)
        {
            var effects = attack.GetEffect(state);
            modifiers.AddRange(effects.UnitModifications);
            spawns.AddRange(effects.UnitSpawns);

        }
        return new UnitBattleEffects(modifiers, spawns);
    }
}

public enum Side
{
    Left,
    Right
}

public class BattleUnitAttack
{
    private BattleUnitState unit;

    public UnitBattleEffects GetEffect(BattleState battleState)
    {
        throw new NotImplementedException();
    }
}

public class PositioningState
{
    public float Position { get; }

    public bool EscapedBattlefield { get; }
}

public class UnitBattleEffects
{
    public IEnumerable<UnitStateModifier> UnitModifications { get; }
    public IEnumerable<BattleUnitState> UnitSpawns { get; }

    public UnitBattleEffects(IEnumerable<UnitStateModifier> unitModifications, 
        IEnumerable<BattleUnitState> unitSpawns)
    {
        UnitModifications = unitModifications;
        UnitSpawns = unitSpawns;
    }
}

public interface IEffectSource { }

public class UnitStateModifier
{
    public IEffectSource Source { get; }

    public BattleUnit Target { get; }
    
    public float HitpointsModification { get; }
}