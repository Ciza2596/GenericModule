namespace CizaAchievementModule
{
    public interface IRuleInfo
    {
        bool IsEnable { get; }

        IConditionInfo[] ConditionInfos { get; }
    }
}