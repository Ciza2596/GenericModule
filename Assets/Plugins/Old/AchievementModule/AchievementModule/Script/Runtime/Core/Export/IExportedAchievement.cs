using System.Collections.Generic;

namespace CizaAchievementModule
{
    public interface IExportedAchievement
    {
        Dictionary<string, ExportedStat> ExportedStatMapByStatDataId { get; }

        Dictionary<string, bool> IsUnlockedMapByAchievementDataId { get; }
    }
}