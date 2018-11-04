using System.Collections.Generic;

public class GameTurnMoves
{
    public List<MergerMove> Mergers { get; }
    public List<UpgradeMove> Upgrades { get; }
    public List<ArmyMove> ArmyMoves { get; }
    public List<RallyPointChange> RallyChanges { get; }

    public GameTurnMoves(IEnumerable<PlayerMove> movesStack)
    {
        Mergers = new List<MergerMove>();
        Upgrades = new List<UpgradeMove>();
        ArmyMoves = new List<ArmyMove>();
        RallyChanges = new List<RallyPointChange>();

        foreach (PlayerMove move in movesStack)
        {
            switch (move.Category)
            {
                case PlayerMove.MoveCategory.Merger:
                    Mergers.Add(move as MergerMove);
                    break;
                case PlayerMove.MoveCategory.Upgrade:
                    Upgrades.Add(move as UpgradeMove);
                    break;
                case PlayerMove.MoveCategory.ArmyMove:
                    ArmyMoves.Add(move as ArmyMove);
                    break;
                case PlayerMove.MoveCategory.RallyPointChange:
                default:
                    RallyChanges.Add(move as RallyPointChange);
                    break;
            }
        }
    }
}
