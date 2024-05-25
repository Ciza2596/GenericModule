namespace CizaAudioModule
{
    public interface IRestrictContinuousPlay
    {
        bool IsEnable { get; }

        float Duration { get; }

        int MaxConsecutiveCount { get; }
    }
}