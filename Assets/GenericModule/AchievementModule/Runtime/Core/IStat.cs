namespace CizaAchievementModule
{
    public interface IStat : IStatReadModel
    {
        void SetCurrent(float value);

        void SetIsUnlocked(bool isUnlocked);
    }
}