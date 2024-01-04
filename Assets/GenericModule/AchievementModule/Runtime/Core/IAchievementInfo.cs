namespace CizaAchievementModule
{
    public interface IAchievementInfo
    {
        string DataId { get; }

        IRuleInfo[] RuleInfos { get; }
    }
}