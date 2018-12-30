using System.Collections.Generic;

public class GameTurnMoves
{
    private readonly List<AttackMove> _armyMoves;
    public IEnumerable<AttackMove> ArmyMoves { get { return _armyMoves; } }

    private readonly List<UpgradeMove> _upgrades;
    public IEnumerable<UpgradeMove> Upgrades { get { return _upgrades; } }

    private readonly List<MergerMove> _mergers;
    public IEnumerable<MergerMove> Mergers { get { return _mergers; } }

    public GameTurnMoves(IEnumerable<PlayerMove> movesStack)
    {
        _mergers = new List<MergerMove>();
        _upgrades = new List<UpgradeMove>();
        _armyMoves = new List<AttackMove>();

        foreach (PlayerMove move in movesStack)
        {
            switch (move.Category)
            {
                case PlayerMove.MoveCategory.Merger:
                    _mergers.Add(move as MergerMove);
                    break;
                case PlayerMove.MoveCategory.Upgrade:
                    _upgrades.Add(move as UpgradeMove);
                    break;
                case PlayerMove.MoveCategory.Attack:
                default:
                    _armyMoves.Add(move as AttackMove);
                    break;
            }
        }
    }
}
