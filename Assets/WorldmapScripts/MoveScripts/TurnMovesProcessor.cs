using System;
using System.Collections.Generic;
using System.Linq;

public class TurnMovesProcessor
{
    private readonly MainGameManager _mainManager;
    public PlayerMoveBuilder ActiveBuilder { get; private set; }

    private IEnumerable<PlayerMoveBuilder> _builders;
    
    public TurnMovesProcessor(MainGameManager mainManager, InteractionManager interactionManager, IEnumerable<PlayerSetup> playerSetups)
    {
        _mainManager = mainManager;
        _builders = playerSetups.Select(item => new PlayerMoveBuilder(this, item.Faction)).ToArray();
        interactionManager.PlayerFaction.ValueChangedEvent += OnPlayerFactionChanged;
        SwitchActiveFaction(playerSetups.First().Faction);
    }

    private void OnPlayerFactionChanged(Faction oldValue, Faction newValue)
    {
        SwitchActiveFaction(newValue);
    }

    internal PlayerMoveBuilder GetMoveBuilderFor(Faction faction)
    {
        return _builders.First(item => item.PlayerFaction == faction);
    }

    public void RenewBuilders(IEnumerable<Faction> factions)
    {
        foreach (PlayerMoveBuilder builder in _builders)
        {
            builder.Renew();
        }
    }

    internal void OnMoveSubmitted()
    {
        if(_builders.All(item => item.Submitted))
        {
            GameTurnMoves moves = GetGameTurnMoves();
            _mainManager.AdvanceGame(moves);
        }
    }

    private GameTurnMoves GetGameTurnMoves()
    {
        List<PlayerMove> moves = new List<PlayerMove>();
        foreach (PlayerMoveBuilder builder in _builders)
        {
            moves.AddRange(builder.GetMoves());
        }
        return new GameTurnMoves(moves);
    }

    public void SwitchActiveFaction(Faction faction)
    {
        ActiveBuilder = _builders.First(item => item.PlayerFaction == faction);
    }
}
