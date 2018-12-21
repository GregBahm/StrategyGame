public class TimelineInteraction
{
    public float DisplayTime { get; private set; }

    private readonly InteractionManager _interactionManager;

    public TimelineInteraction(InteractionManager interactionManager)
    {
        _interactionManager = interactionManager;
    }
}
