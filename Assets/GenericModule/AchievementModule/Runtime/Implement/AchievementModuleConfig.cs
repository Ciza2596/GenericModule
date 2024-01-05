using System;
using System.Linq;
using UnityEngine;

namespace CizaAchievementModule.Implement
{
    [CreateAssetMenu(fileName = "AchievementModuleConfig", menuName = "Ciza/AchievementModule/AchievementModuleConfig")]
    public class AchievementModuleConfig : ScriptableObject, IAchievementModuleConfig
    {
        [SerializeField]
        private StatInfo[] _statInfos;

        [SerializeField]
        private AchievementInfo[] _achievementInfos;

        public Type StatType => typeof(Stat);

        public IStatInfo[] StatInfos => _statInfos != null ? _statInfos.Cast<IStatInfo>().ToArray() : Array.Empty<IStatInfo>();

        public IAchievementInfo[] AchievementInfos => _achievementInfos != null ? _achievementInfos.Cast<IAchievementInfo>().ToArray() : Array.Empty<IAchievementInfo>();
    }
}