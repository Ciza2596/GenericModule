using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAchievementModule
{
    [Serializable]
    public class ExportedAchievement: IExportedAchievement
    {
        [SerializeField]
        private Dictionary<string, ExportedStat> _exportedStatMapByStatDataId;

        [SerializeField]
        private Dictionary<string, bool> _isUnlockedMapByAchievementDataId;

        [Preserve]
        public ExportedAchievement() : this(new Dictionary<string, ExportedStat>(), new Dictionary<string, bool>()) { }

        [Preserve]
        public ExportedAchievement(Dictionary<string, ExportedStat> exportedStatMapByStatDataId, Dictionary<string, bool> isUnlockedMapByAchievementDataId)
        {
            _exportedStatMapByStatDataId = exportedStatMapByStatDataId;
            _isUnlockedMapByAchievementDataId = isUnlockedMapByAchievementDataId;
        }

        public Dictionary<string, ExportedStat> ExportedStatMapByStatDataId => _exportedStatMapByStatDataId.ToDictionary(exportedStatMapByStatDataId => exportedStatMapByStatDataId.Key, exportedStatMapByStatDataId => exportedStatMapByStatDataId.Value);

        public Dictionary<string, bool> IsUnlockedMapByAchievementDataId => _isUnlockedMapByAchievementDataId.ToDictionary(isUnlockedMapByAchievementDataId => isUnlockedMapByAchievementDataId.Key, isUnlockedMapByAchievementDataId => isUnlockedMapByAchievementDataId.Value);
    }
}