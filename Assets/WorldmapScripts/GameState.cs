using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class GameState
{
    private readonly ReadOnlyCollection<Province> _provinces;
    private readonly ReadOnlyCollection<Army> _armies;

    public GameState(IEnumerable<Province> provinces, IEnumerable<Army> armies)
    {
        _provinces = provinces.ToList().AsReadOnly();
        _armies = armies.ToList().AsReadOnly();
    }

    public GameState GetNextState(GameTurnMoves moves)
    {
        List<Province> newProvinces = new List<Province>(_provinces);
        List<Army> newArmies = new List<Army>(_armies);
        HandleArmyMoves(moves);
        HandleMergers(moves);
        HandleUpgrades(moves);
        HandleRallyChanes(moves);
        return new GameState(newProvinces, newArmies);
    }

    private void HandleRallyChanes(GameTurnMoves turn)
    {
        // Need to make sure they still control the rallying forces
        foreach (RallyPointChange rallyChange in turn.RallyChanges)
        {
            throw new NotImplementedException();
        }
    }

    private void HandleArmyMoves(GameTurnMoves turn)
    {
        // Need to handle situations where two armys collide
        foreach (ArmyMove armyMove in turn.ArmyMoves)
        {
            throw new NotImplementedException();
        }
    }

    private void HandleUpgrades(GameTurnMoves turn)
    {
        // Need to make sure they're not upgrading a province they no longer own
        foreach (UpgradeMove upgrade in turn.Upgrades)
        {
            throw new NotImplementedException();
        }
    }

    private void HandleMergers(GameTurnMoves turn)
    {
        // Need to make sure they're not merging a province they no longer own
        foreach (MergerMove item in turn.Mergers)
        {
            throw new NotImplementedException();
        }
    }
}
