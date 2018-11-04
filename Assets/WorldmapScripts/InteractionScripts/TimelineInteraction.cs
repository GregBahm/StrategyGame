public class TimelineInteraction
{
    public ObservableProperty<float> MasterGameTime { get; set; }

    public TimelineInteraction()
    {
        MasterGameTime = new ObservableProperty<float>(0);
    }
}
