public class Army
{
    public Faction Owner { get; }
    public Army(Faction owner)
    {
        Owner = owner;
    }
}
