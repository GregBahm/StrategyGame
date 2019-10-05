using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace CardCombat
{
    public class CardPrototypeOptions
    {
        public ReadOnlyCollection<CardPrototypeOption> Options { get; }

        public IEnumerable<Card> GetCards()
        {
            return Options.Where(item => item.IsSelected).Select(item => item.Card).ToList();
        }
    }

    public class CardPrototypeOption
    {
        public bool IsSelected { get; set; }
        public Card Card { get; }
    }

    public class CardCombatPrototype : MonoBehaviour
    {
        public CardPrototypeOptions SideAOptions { get; }
        public CardPrototypeOptions SideBOptions { get; }

        private CardBattleState GetInitialCards(CardPrototypeOptions sideA, CardPrototypeOptions sideB)
        {
            return GetInitialState(SideAOptions.GetCards(), SideBOptions.GetCards());
        }

        private CardBattleState GetInitialState(IEnumerable<Card> sideACards, IEnumerable<Card> sideBCards)
        {

        }
    }

    public class CardBattle
    {
        public const int TurnLimit = 2000;

        public ReadOnlyCollection<CardBattleState> History { get; }
        public BattleStatus Outcome { get; }

        public CardBattle(CardBattleState initialState)
        {
            History = GetHistory(initialState);
            Outcome = History.Last().Status;
        }

        private ReadOnlyCollection<CardBattleState> GetHistory(CardBattleState initialState)
        {
            List<CardBattleState> ret = new List<CardBattleState>() { initialState };
            CardBattleState currentState = initialState;
            while (currentState.Status == BattleStatus.Undecided && ret.Count < TurnLimit)
            {
                currentState = currentState.GetNextState();
                ret.Add(currentState);
            }

            return ret.AsReadOnly();
        }
    }

    public class CardBattleState
    {
        public ReadOnlyCollection<Card> PriorCards { get; }
        public Card CurrentCard { get; }
        public ReadOnlyCollection<Card> RemainingCards { get; }
        public BattleStatus Status { get; }

        public CardBattleState GetNextState()
        {

        }
    }

    public class CardDescription
    {
        public string Name { get; }
    }

    public class Card
    {
        public CardFaction Faction { get; }
        public CardDescription Description { get; }
    }

    public enum BattleStatus
    {
        SideAWon,
        SideBWon,
        Tie,
        Undecided
    }

    public enum CardFaction
    {
        SideA,
        SideB,
        Neutral
    }

    public enum Rank
    {
        Front,
        Mid,
        Back
    }

    public class TargetableCard : Card
    {
        public int MaxHitpoints { get; }
        public int CurrentHitpoints { get; }

        public int Attack { get; }
        public int Defense { get; }

        public int MaxMoral { get; }
        public int RemainingMoral { get; }

        public int Initiative { get; }

        public Rank Rank { get; }
    }
}