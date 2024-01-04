using System.Collections.Generic;

namespace CizaAchievementModule
{
    public interface IAchievementModuleConfig
    {
        string[] DefinedDataIds { get; }


        IDictionary<string, IAchievementInfo> CreateAchievementInfoMapByAchievementDataId();
    }
}