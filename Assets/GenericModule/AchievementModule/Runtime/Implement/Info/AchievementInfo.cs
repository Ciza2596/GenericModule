using System;
using System.Linq;
using UnityEngine;

namespace CizaAchievementModule
{
    [Serializable]
    public class AchievementInfo : IAchievementInfo
    {
        [SerializeField]
        private string _dataId;

        [Space]
        [SerializeField]
        private RuleInfo[] _ruleInfos;

        public string DataId => _dataId;

        public IRuleInfo[] RuleInfos => _ruleInfos != null ? _ruleInfos.Cast<IRuleInfo>().ToArray() : Array.Empty<IRuleInfo>();
    }
}