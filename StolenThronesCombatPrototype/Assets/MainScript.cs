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
            BattleState selfModified = currentState.GetSelfUpdated();
            IEnumerable<UnitStateModifier> attacks = selfModified.GetAttackModifiers().ToArray();
            BattleState finalState = selfModified.GetWithAttacksApplied(attacks);
            BattleRound round = new BattleRound(currentState, selfModified, attacks, finalState);
            ret.Add(round);

            currentState = finalState;
        }
        return ret;
    }
}

public class BattleRound
{

    public BattleState InitialState { get; }
    public BattleState SelfModified { get; }
    public IEnumerable<UnitStateModifier> Modifiers { get; }
    public BattleState FinalState { get; }
    public BattleRound(BattleState initialState, 
        BattleState selfModified, 
        IEnumerable<UnitStateModifier> modifiers, 
        BattleState finalState)
    {
        InitialState = initialState;
        SelfModified = selfModified;
        Modifiers = modifiers;
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

    public BattleState GetSelfUpdated()
    {
        BattleStageSide leftSelfUpdate = GetSelfUpdates(LeftSide, RightSide);
        BattleStageSide rightSelfUpdates = GetSelfUpdates(RightSide, LeftSide);
        return new BattleState(leftSelfUpdate, rightSelfUpdates);
    }

    public IEnumerable<UnitStateModifier> GetAttackModifiers()
    {

    }

    public BattleState GetWithAttacksApplied(IEnumerable<UnitStateModifier> modifiers)
    {
        Dictionary<BattleUnit, BattleUnitState> leftDictionary = LeftSide.Units.ToDictionary(item => item.Id, item => item);
        Dictionary<BattleUnit, BattleUnitState> rightDictionary = RightSide.Units.ToDictionary(item => item.Id, item => item);

        foreach (UnitStateModifier modifier in modifiers)
        {
            if(leftDictionary.ContainsKey(modifier.Target))
            {
                ApplyAttack(modifier, leftDictionary);
            }
            else if(rightDictionary.ContainsKey(modifier.Target))
            {
                ApplyAttack(modifier, rightDictionary);
            }
            else
            {
                throw new Exception("Can't find the target of an attack modifier");
            }
        }
    }

    private void ApplyAttack(UnitStateModifier modifier, Dictionary<BattleUnit, BattleUnitState> unitsDictionary)
    {
        throw new NotImplementedException();
    }

    private BattleStageSide GetSelfUpdates(BattleStageSide self, BattleStageSide opponent)
    {
        throw new NotImplementedException();
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
    public float RemainingHitpoints { get; }

    public bool IsAlive { get { return RemainingHitpoints > 0 && !Positiong.LeftBattlefield; } }
    
    public IEnumerable<AttackState> Attacks { get; }

    public PositioningState Positiong { get; }

    public BattleUnitState GetNextSelf()
    {
        throw new NotImplementedException();
    }

    public BattleUnitState GetWithModifierApplied(UnitStateModifier modifier)
    {

    }
}

public class PositioningState
{
    public float Position { get; }

    public bool LeftBattlefield { get; }
}

public class AttackState
{
    public 
}

public class AttackEffects
{
    public IEnumerable<UnitStateModifier> AllyStateModifications { get; }
    public IEnumerable<BattleUnitState> AllySpawns { get; }

    public IEnumerable<UnitStateModifier> EnemyModifications { get; }
    public IEnumerable<BattleUnitState> EnemySpawns { get; }
}

public interface IEffectSource { }

public class UnitStateModifier
{
    public IEffectSource Source { get; }

    public BattleUnit Target { get; }
    
    public float HitpointsModification { get; }
}