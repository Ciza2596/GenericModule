namespace CizaTimerModule
{
    public interface ITimerReadModel
    {
        bool IsOnce { get; }

        bool IsPlayed { get; }
        float Duration { get; }
        float Time { get; }
    }
}