namespace CizaTimerModule
{
    public interface ITimerReadModel
    {
        bool IsOnce { get; }
        float Duration { get; }
        float Time { get; }
    }
}