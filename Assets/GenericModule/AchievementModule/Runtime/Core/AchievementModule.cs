using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace CizaAchievementModule
{
    public class AchievementModule
    {
        public const float True = 1;
        public const float False = 0;

        private readonly IAchievementModuleConfig _config;

        private readonly Dictionary<string, IStat> _statMapByStatDataId = new Dictionary<string, IStat>();
        private readonly Dictionary<string, bool> _isUnlockedMapByAchievementDataId = new Dictionary<string, bool>();

        // AchievementDataId
        public event Action<string> OnUnlocked;

        private bool _isInitializing;
        private bool _isInitialized;

        public bool IsInitialized => _isInitialized && !_isInitializing;

        public string[] AllStatDataIds => _statMapByStatDataId.Keys.ToArray();
        public string[] AllAchievementDataIds => _isUnlockedMapByAchievementDataId.Keys.ToArray();

        public bool TryGetStatReadModel(string statDataId, out IStatReadModel statReadModel)
        {
            if (!_statMapByStatDataId.TryGetValue(statDataId, out var stat))
            {
                statReadModel = null;
                return false;
            }

            statReadModel = stat;
            return true;
        }

        public bool TryGetIsAchievementUnlocked(string achievementDataId, out bool isUnlocked) =>
            _isUnlockedMapByAchievementDataId.TryGetValue(achievementDataId, out isUnlocked);

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
                Assert.IsFalse(_statMapByStatDataId.ContainsKey(stat.DataId), $"[AchievementModule::Initialize] Stat: {stat.DataId} is already created.");
                _statMapByStatDataId.Add(statInfo.DataId, stat);
            }

            foreach (var achievementInfo in _config.AchievementInfos)
            {
                Assert.IsFalse(_isUnlockedMapByAchievementDataId.ContainsKey(achievementInfo.DataId), $"[AchievementModule::Initialize] IsUnlocked: {achievementInfo.DataId} is already created.");
                _isUnlockedMapByAchievementDataId.Add(achievementInfo.DataId, false);
            }

            _isInitializing = false;
        }

        public void Release()
        {
            if (!_isInitialized)
                return;

            _statMapByStatDataId.Clear();
            _isInitialized = false;
        }

        // StatDataId, Current 
        public ExportedAchievement Export()
        {
            if (!IsInitialized)
                return new ExportedAchievement();

            return new ExportedAchievement(CreateExportedStatMapByDataId(), CreateExportedIsUnlockedMapByAchievementDataId());
        }

        public void Import(IExportedAchievement exportedAchievement)
        {
            if (!IsInitialized || exportedAchievement == null)
                return;

            foreach (var exportedStat in exportedAchievement.ExportedStatMapByStatDataId.Values.ToArray())
            {
                if (!_statMapByStatDataId.TryGetValue(exportedStat.DataId, out var stat))
                    continue;

                stat.SetCurrent(exportedStat.Current);
            }

            foreach (var pair in exportedAchievement.IsUnlockedMapByAchievementDataId)
            {
                if (!_isUnlockedMapByAchievementDataId.ContainsKey(pair.Key))
                    continue;

                _isUnlockedMapByAchievementDataId[pair.Key] = pair.Value;
            }

            Refresh();
        }

        public void Refresh()
        {
            if (!IsInitialized)
                return;

            CheckAnyIsAchievementUnlocked(false);
        }

        public void SetStat(string statDataId, float value)
        {
            if (!IsInitialized || !_statMapByStatDataId.TryGetValue(statDataId, out var stat))
                return;

            stat.SetCurrent(value);

            CheckAnyIsAchievementUnlocked(true);
        }

        public void AddStat(string statDataId, float value)
        {
            if (!IsInitialized || !_statMapByStatDataId.TryGetValue(statDataId, out var stat))
                return;

            SetStat(stat.DataId, stat.Current + value);
        }

        public void SetStatBeTrue(string statDataId) =>
            SetStat(statDataId, True);

        public void SetStatBeFalse(string statDataId) =>
            SetStat(statDataId, False);

        private void CheckAnyIsAchievementUnlocked(bool isFirstUnlocked)
        {
            foreach (var achievementDataId in _isUnlockedMapByAchievementDataId.Keys.ToArray())
                CheckIsAchievementUnlocked(isFirstUnlocked, achievementDataId);
        }

        private void CheckIsAchievementUnlocked(bool isFirstUnlocked, string achievementDataId)
        {
            if (CheckIsUnlocked(isFirstUnlocked, achievementDataId))
            {
                _isUnlockedMapByAchievementDataId[achievementDataId] = true;
                OnUnlocked?.Invoke(achievementDataId);
            }
        }

        private bool CheckIsUnlocked(bool isFirstUnlocked, string achievementDataId)
        {
            if (!_isUnlockedMapByAchievementDataId.TryGetValue(achievementDataId, out var isUnlocked))
                return false;

            if (isFirstUnlocked && isUnlocked)
                return false;

            var achievementInfo = _config.AchievementInfos.FirstOrDefault(achievementInfo => achievementInfo.DataId == achievementDataId);
            if (achievementInfo == null)
                return false;

            foreach (var ruleInfo in achievementInfo.RuleInfos)
                if (ruleInfo.IsEnable && CheckIsSucceed(ruleInfo.ConditionInfos))
                    return true;

            return false;
        }

        private bool CheckIsSucceed(IConditionInfo[] conditionInfos)
        {
            var isConditionCompletedCount = 0;
            var isConditionEnableCount = 0;

            foreach (var conditionInfo in conditionInfos)
            {
                if (!conditionInfo.IsEnable)
                    continue;

                isConditionEnableCount++;

                if (TryGetStatReadModel(conditionInfo.StatDataId, out var statReadModel) && statReadModel.Current >= conditionInfo.Value)
                    isConditionCompletedCount++;
            }

            return isConditionCompletedCount == isConditionEnableCount;
        }


        private Dictionary<string, ExportedStat> CreateExportedStatMapByDataId()
        {
            var exportedStatMapByDataId = new Dictionary<string, ExportedStat>();
            foreach (var stat in _statMapByStatDataId.Values.ToArray())
                exportedStatMapByDataId.Add(stat.DataId, new ExportedStat(stat.DataId, stat.Current));
            return exportedStatMapByDataId;
        }

        private Dictionary<string, bool> CreateExportedIsUnlockedMapByAchievementDataId() =>
            _isUnlockedMapByAchievementDataId.ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}