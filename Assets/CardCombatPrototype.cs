using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class CardCombatPrototype : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class CardBattleState
{
    public ReadOnlyCollection<Card> PriorCards { get; }
    public Card CurrentCard { get; }
    public ReadOnlyCollection<Card> RemainingCards { get; }
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

public enum CardBattleStateOutcome
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

    public Rank Rank { get; }
}