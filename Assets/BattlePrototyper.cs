using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BattlePrototyper : MonoBehaviour 
{
    private void Start()
    {
        List<UnitStateBuilder> attackingUnits = GetPrototypeAttackers();
        List<UnitStateBuilder> defendingUnits = GetPrototypeDefenders();
        BattleRound battleRound = GetBattleRound(attackingUnits, defendingUnits);
        DisplayBattleRound(battleRound);
    }

    private void DisplayBattleRound(BattleRound battleRound)
    {
        foreach (UnitState state in battleRound.AttackingUnits.Concat(battleRound.DefendingUnits))
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.position = new Vector3(state.Attributes.Position.XPos, 0, state.Attributes.Position.YPos);
            obj.name = state.Description.Name;
        }
    }

    private BattleRound GetBattleRound(List<UnitStateBuilder> attackingUnits, List<UnitStateBuilder> defendingUnits)
    {
        UnitState[] attackingState = attackingUnits.Select(item => item.AsReadonly()).ToArray();
        UnitState[] defendingState = defendingUnits.Select(item => item.AsReadonly()).ToArray();
        BattleStatus status = GetBattleStatus(attackingState, defendingState);
        return new BattleRound(attackingState, defendingState, status);
    }

    private BattleStatus GetBattleStatus(UnitState[] attackingState, UnitState[] defendingState)
    {
        if(attackingState.Any())
        {
            if(defendingState.Any())
            {
                return BattleStatus.Ongoing;
            }
            return BattleStatus.AttackersVictorious;
        }
        {
            return BattleStatus.DefendersVictorious;
        }
    }

    private List<UnitStateBuilder> GetPrototypeDefenders()
    {
        List<UnitStateBuilder> ret = new List<UnitStateBuilder>();
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(10, i * 3 + 40));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(5, i * 3 + 10));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetKnight(10, i * 3));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetKnight(10, i * 3 + 100));
        }
        return ret;
    }

    private List<UnitStateBuilder> GetPrototypeAttackers()
    {
        List<UnitStateBuilder> ret = new List<UnitStateBuilder>();
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetSwordsman(90, i * 3 + 40));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(95, i * 3 + 40));
        }
        for (int i = 0; i < 10; i++)
        {
            ret.Add(UnitTemplates.GetArcher(97, i * 3 + 40));
        }
        for (int i = 0; i < 6; i++)
        {
            ret.Add(UnitTemplates.GetTroll(85, i * 3 + 60));
        }
        return ret;
    }
}