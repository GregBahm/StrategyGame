public class BattalionStateModifier
{
    public BattalionEffector Source { get; }

    public BattalionIdentifier Target { get; }

    public BattalionAttribute Attribute { get; }

    public int Modifier { get; }
    
    public BattalionStateModifier(BattalionEffector source, 
        BattalionIdentifier target, 
        BattalionAttribute attribute, 
        int modifier)
    {
        Source = source;
        Target = target;
        Attribute = attribute;
        Modifier = modifier;
    }

}