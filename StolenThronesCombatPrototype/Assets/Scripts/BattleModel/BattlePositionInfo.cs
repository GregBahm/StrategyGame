public class BattlePositionInfo
{
    public BattlePosition InitialPosition { get; }
    public BattlePosition EffectivePosition { get; }
    public int IndexWithinPosition { get; }

    public BattlePositionInfo(BattlePosition initialPosition, BattlePosition effectivePosition, int indexWithinPosition)
    {
        InitialPosition = initialPosition;
        EffectivePosition = effectivePosition;
        IndexWithinPosition = indexWithinPosition;
    }
}
