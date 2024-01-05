using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace CizaAchievementModule
{
    public class AchievementModule
    {
        private readonly IAchievementModuleConfig _config;

        private readonly Dictionary<string, IStat> _statMapByDataId = new Dictionary<string, IStat>();

        // AchievementDataId
        public event Action<string> OnAchievementUnlocked;

        private bool _isInitializing;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized && !_isInitializing;

        public bool TryGetStatReadModel(string dataId, out IStatReadModel statReadModel)
        {
            if (!_statMapByDataId.TryGetValue(dataId, out var stat))
            {
                statReadModel = null;
                return false;
            }

            statReadModel = stat;
            return true;
        }

        public AchievementModule(IAchievementModuleConfig achievementModuleConfig) =>
            _config = achievementModuleConfig;

        public void Initialize()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            _isInitializing = true;
            foreach (var statInfo in _config.StatInfos)
            {
                var stat = Activator.CreateInstance(_config.StatType, statInfo.DataId, statInfo.Min, statInfo.Max) as IStat;
                Assert.IsNotNull(stat, "[AchievementModule::Initialize] Stat is null. Please check config statType.");
                Assert.IsFalse(_statMapByDataId.ContainsKey(stat.DataId), $"[AchievementModule::Initialize] Stat: {stat.DataId} is already created.");
                _statMapByDataId.Add(statInfo.DataId, stat);
            }

            _isInitializing = false;
        }

        public void Release()
        {
            if (!_isInitialized)
                return;

            _statMapByDataId.Clear();
            _isInitialized = false;
        }

        // StatDataId, Current 
        public Dictionary<string, ExportedStat> Export()
        {
            var exportedStatMapByDataId = new Dictionary<string, ExportedStat>();
            if (!IsInitialized)
                return exportedStatMapByDataId;

            foreach (var statReadModel in _statMapByDataId.Values.ToArray())
                exportedStatMapByDataId.Add(statReadModel.DataId, new ExportedStat(statReadModel.DataId, statReadModel.Current, statReadModel.IsUnlocked));
            return exportedStatMapByDataId;
        }

        public void Import(Dictionary<string, ExportedStat> exportedStatMapByDataId)
        {
            if (!IsInitialized)
                return;

            foreach (var exportedStat in exportedStatMapByDataId.Values.ToArray())
            {
                if (!_statMapByDataId.TryGetValue(exportedStat.DataId, out var stat))
                    continue;

                stat.SetCurrent(exportedStat.Current);
                stat.SetIsUnlocked(exportedStat.IsUnlocked);
            }
        }

        public void SetStat(string statDataId, float value)
        {
            if (!IsInitialized || !_statMapByDataId.TryGetValue(statDataId, out var stat))
                return;

            stat.SetCurrent(value);

            if (CheckIsFirstUnlocked(stat, out var achievementDataId))
                OnAchievementUnlocked?.Invoke(achievementDataId);
        }

        public void AddStat(string statDataId, float value)
        {
            if (!IsInitialized || !_statMapByDataId.TryGetValue(statDataId, out var stat))
                return;

            SetStat(stat.DataId, stat.Current + value);
        }

        private bool CheckIsFirstUnlocked(IStat stat, out string achievementDataId)
        {
            if (stat.IsUnlocked)
            {
                achievementDataId = string.Empty;
                return false;
            }

            achievementDataId = string.Empty;
            return true;
        }
    }
}