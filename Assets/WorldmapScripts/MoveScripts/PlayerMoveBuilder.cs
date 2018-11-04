using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveBuilder
{
    private readonly TurnMovesProcessor _moveProcessor;

    public Faction PlayerFaction { get; }

    public bool Submitted { get; private set; }

    public PlayerMoveBuilder(TurnMovesProcessor moveProcessor, Faction playerFaction)
    {
        _moveProcessor = moveProcessor;
        PlayerFaction = playerFaction;
    }

    public void SubmitMove()
    {
        Submitted = true;
        _moveProcessor.OnMoveSubmitted();
    }

    internal IEnumerable<PlayerMove> GetMoves()
    {
        throw new NotImplementedException();
    }
}
