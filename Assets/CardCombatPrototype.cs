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
            IEnumerable<Card> cards = SideAOptions.GetCards().Concat(SideBOptions.GetCards());
            ReadOnlyCollection<Card> orderedCards = CardQueueBuilder.GetCardQueue(cards);
            return new CardBattleState(orderedCards);
        }
    }

    /// <summary>
    /// The idea here is to organize the two decks such that: 
    ///     - higher initiative cards always go before lower initiative cards
    ///     - cards of equal initiative go in random order, but back and forth between the two sides as evenly as possible, to minimize random advantage 
    /// </summary>
    public static class CardQueueBuilder
    {
        public static ReadOnlyCollection<Card> GetCardQueue(IEnumerable<Card> cards)
        {
            IEnumerable<IGrouping<int, Card>> grouped = cards.GroupBy(item => item.Initiative);
            IOrderedEnumerable<IGrouping<int, Card>> ordered = grouped.OrderBy(item => item.First().Initiative);

            List<Card> ret = new List<Card>();
            foreach (IGrouping<int, Card> item in ordered)
            {
                List<Card> sortedCardSet = GetSortedEqualInitiative(item);
                ret.AddRange(sortedCardSet);
            }
            return ret.AsReadOnly();
        }

        private static List<Card> GetSortedEqualInitiative(IGrouping<int, Card> cards)
        {
            List<Card> sideA = new List<Card>();
            List<Card> sideB = new List<Card>();
            foreach (Card item in cards)
            {
                if (item.Faction == CardFaction.SideA)
                {
                    sideA.Add(item);
                }
                else
                {
                    sideB.Add(item);
                }
            }
            return GetSortedEqualInitiative(sideA, sideB);
        }

        private static List<Card> GetSortedEqualInitiative(List<Card> sideA, List<Card> sideB)
        {
            List<Card> ret = new List<Card>();
            Shuffle(sideA);
            Shuffle(sideB);
            int shorterSide = Mathf.Min(sideA.Count, sideB.Count);
            if (shorterSide == 0)
            {
                ret.AddRange(sideA);
                ret.AddRange(sideB);
                return ret;
            }
            int sideAChunkSize = Mathf.FloorToInt(sideA.Count / shorterSide);
            int sideBChunkSize = Mathf.FloorToInt(sideB.Count / shorterSide);
            for (int i = 0; i < shorterSide; i++)
            {
                for (int sideAChunkIndex = 0; sideAChunkIndex < sideAChunkSize; sideAChunkIndex++)
                {
                    ret.Add(sideA[sideA.Count - 1]);
                    sideA.RemoveAt(sideA.Count - 1);
                }
                for (int sideBChunkIndex = 0; sideBChunkIndex < sideBChunkSize; sideBChunkIndex++)
                {
                    ret.Add(sideB[sideB.Count - 1]);
                    sideB.RemoveAt(sideB.Count - 1);
                }
            }
            ret.AddRange(sideA);
            ret.AddRange(sideB);
            return ret;
        }

        private static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Mathf.FloorToInt(UnityEngine.Random.value * (n + 1));
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
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
        public ReadOnlyCollection<Card> Queue { get; }
        public BattleStatus Status { get; }

        public CardBattleState(ReadOnlyCollection<Card> queue)
        {
            Queue = queue;
            Status = GetStatus();
        }

        private BattleStatus GetStatus()
        {
            bool sideASurvives = false;
            bool sideBSurvives = false;
            foreach (Card card in Queue)
            {
                CombatantCard soldier = card as CombatantCard;
                if(soldier != null && soldier.CanStillFight)
                {
                    if(soldier.Faction == CardFaction.SideA)
                    {
                        sideASurvives = true;
                    }
                    else
                    {
                        sideBSurvives = true;
                    }
                }
            }
            return GetStatus(sideASurvives, sideBSurvives);
        }

        private BattleStatus GetStatus(bool sideASurvives, bool sideBSurvives)
        {
            if (sideASurvives)
            {
                return sideBSurvives ? BattleStatus.Undecided : BattleStatus.SideAWon;
            }
            else if (sideBSurvives)
            {
                return BattleStatus.SideBWon;
            }
            return BattleStatus.Tie;
        }

        public CardBattleState GetNextState()
        {
            Card currentCard = Queue.First();
            ReadOnlyCollection<Card> newQueue = currentCard.GetApplied(Queue);
            return new CardBattleState(newQueue);
        }
    }

    public class CardIdentity
    {
        public string Name { get; }
    }

    public abstract class Card
    {
        public CardFaction Faction { get; }

        public CardIdentity Identity { get; }

        public int Initiative { get; }

        public Card(CardFaction faction, CardIdentity identity, int initiative)
        {
            Faction = faction;
            Identity = identity;
            Initiative = initiative;
        }

        public abstract ReadOnlyCollection<Card> GetApplied(ReadOnlyCollection<Card> queue);
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
    }

    public enum Rank
    {
        Front,
        Mid,
        Back
    }

    public class LeaderCard : Card
    {
        public int Skill { get; }

        public LeaderCard(CardFaction faction, 
            CardIdentity identity, 
            int initiative,
            int skill)
            : base(faction, identity, initiative)
        {
            Skill = skill;
        }

        public override ReadOnlyCollection<Card> GetApplied(ReadOnlyCollection<Card> queue)
        {
            List<Card> ret = queue.ToList();
            ret.RemoveAt(0);
            for (int i = 0; i < ret.Count; i++)
            {
                if(ret[i].Faction == Faction)
                {
                    CombatantCard combatant = ret[i] as CombatantCard;
                    if (combatant != null)
                    {
                        MutableCombatant mutable = combatant.AsMutable();
                        mutable.Attack += Skill;
                        ret[i] = mutable.AsReadonly();
                    }
                }
            }
            ret.Add(this);
            return ret.AsReadOnly();
        }
    }

    public class SpyCard : Card
    {
        public SpyCard(CardFaction faction, 
            CardIdentity identity, 
            int initiative) 
            : base(faction, identity, initiative)
        { }

        public override ReadOnlyCollection<Card> GetApplied(ReadOnlyCollection<Card> queue)
        {
            throw new NotImplementedException();
        }
    }

    public class CombatantCard : Card
    {
        public int MaxHitpoints { get; }
        public int CurrentHitpoints { get; }

        public int Attack { get; }

        public int Defense { get; }

        public int MaxMoral { get; }
        public int RemainingMoral { get; }

        public Rank Rank { get; }

        public bool CanStillFight
        {
            get
            {
                return CurrentHitpoints > 0 && RemainingMoral > 0;
            }
        }

        public CombatantCard(CardFaction faction, 
            CardIdentity identity, 
            int initiative,
            int maxHitpoints, 
            int currentHitpoints, 
            int attack, 
            int defense, 
            int maxMoral, 
            int remainingMoral, 
            Rank rank)
            :base(faction, identity, initiative)
        {
            MaxHitpoints = maxHitpoints;
            CurrentHitpoints = currentHitpoints;
            Attack = attack;
            Defense = defense;
            MaxMoral = maxMoral;
            RemainingMoral = remainingMoral;
            Rank = rank;
        }

        public override ReadOnlyCollection<Card> GetApplied(ReadOnlyCollection<Card> queue)
        {
            List<Card> ret = queue.ToList();
            ret.RemoveAt(0);
            if(CanStillFight)
            {
                int targetIndex = GetCombatTargetIndex(queue);
                CombatantCard damagedTarget = GetDamagedTarget(ret[targetIndex]);
                ret.RemoveAt(targetIndex);
                ret.Insert(targetIndex, damagedTarget);
            }
            else
            {

            }
            // TODO: Add self back to the end of the queue
            return ret.AsReadOnly();
        }

        private CombatantCard GetDamagedTarget(Card rawTarget)
        {
            CombatantCard castTarget = rawTarget as CombatantCard;
            MutableCombatant target = castTarget.AsMutable();
            throw new NotFiniteNumberException();
            return target.AsReadonly();
        }

        private int GetCombatTargetIndex(ReadOnlyCollection<Card> queue)
        {
            throw new NotImplementedException();
        }

        public MutableCombatant AsMutable()
        {
            return new MutableCombatant()
            {
                Faction = Faction,
                Identity = Identity,
                Initiative = Initiative,
                MaxHitpoints = MaxHitpoints,
                CurrentHitpoints = CurrentHitpoints,
                Attack = Attack,
                Defense = Defense,
                MaxMoral = MaxMoral,
                RemainingMoral = RemainingMoral,
                Rank = Rank
            };
        }
    }

    public class MutableCombatant
    {
        public CardFaction Faction { get; set; }

        public CardIdentity Identity { get; set; }

        public int Initiative { get; set; }

        public int MaxHitpoints { get; set; }

        public int CurrentHitpoints { get; set; }

        public int Attack { get; set; }

        public int Defense { get; set; }

        public int MaxMoral { get; set; }

        public int RemainingMoral { get; set; }

        public Rank Rank { get; set; }

        public CombatantCard AsReadonly()
        {
            return new CombatantCard(Faction,
                Identity,
                Initiative,
                MaxHitpoints,
                CurrentHitpoints,
                Attack,
                Defense,
                MaxMoral,
                RemainingMoral,
                Rank);
        }
    }
}