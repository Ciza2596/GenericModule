using System;
using System.Collections.Generic;

namespace CizaAchievementModule
{
    public class AchievementModule
    {
        private readonly IAchievementModuleConfig _config;

        private readonly Dictionary<string, IStat> _statMapByDataId = new Dictionary<string, IStat>();

        private Dictionary<string, IAchievementInfo> _achievementInfoMapByAchievementDataId;

        // AchievementDataId
        public event Action<string> OnAchievementUnlocked;

        public bool IsInitialized => _achievementInfoMapByAchievementDataId != null;

        public AchievementModule(IAchievementModuleConfig achievementModuleConfig) =>
            _config = achievementModuleConfig;


        public void Initialize() { }

        public void Release() { }
    }
}