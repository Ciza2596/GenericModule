namespace CizaAchievementModule
{
    public interface IStatReadModel
    {
        string DataId { get; }

        float Min { get; }
        float Max { get; }

        float Current { get; }


        bool IsUnlocked { get; }
    }
}