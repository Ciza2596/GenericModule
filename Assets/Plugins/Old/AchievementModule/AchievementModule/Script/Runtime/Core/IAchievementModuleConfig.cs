using System;


namespace CizaAchievementModule
{
    public interface IAchievementModuleConfig
    {
        Type StatType { get; }


        IStatInfo[] StatInfos { get; }

        IAchievementInfo[] AchievementInfos { get; }
    }
}