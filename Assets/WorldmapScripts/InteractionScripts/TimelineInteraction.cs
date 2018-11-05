public class TimelineInteraction
{
    private readonly InteractionManager _interactionManager;

    public TimelineInteraction(InteractionManager interactionManager)
    {
        _interactionManager = interactionManager;
        _interactionManager.MasterGameTime.ValueChangedEvent += OnTimeChanged;
    }

    private void OnTimeChanged(float oldValue, float newValue)
    {
        throw new System.NotImplementedException();
    }
}
