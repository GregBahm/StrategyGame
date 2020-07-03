public class BattalionSpawnEffect
{
    public BattalionIdentifier SpawnSource { get; }
    public BattalionState SpawnedUnit { get; }

    public SpawnPosition Position { get; }

    public BattalionSpawnEffect(BattalionIdentifier spawnSource, 
        BattalionState spawnedUnit, 
        SpawnPosition position)
    {
        SpawnSource = spawnSource;
        SpawnedUnit = spawnedUnit;
        Position = position;
    }

    public enum SpawnPosition
    {
        BeforeSpawnSource,
        AfterSpawnSource,
        First,
        Last
    }
}