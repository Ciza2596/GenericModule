namespace CizaAchievementModule
{
    public interface IConditionInfo
    {
        bool IsEnable { get; }

        string StatDataId { get; }
        float Value { get; }
    }
}