namespace CizaTimerModule
{
    public interface TimerReadModel
    {
        bool IsOnce { get; }
        float TriggerTime { get; }
        float Time { get; }
    }
}