using System;
using System.Linq;
using UnityEngine;

namespace CizaAchievementModule
{
    [Serializable]
    public class RuleInfo : IRuleInfo
    {
        [SerializeField]
        private bool _isEnable;

        [Space]
        [SerializeField]
        private ConditionInfo[] _conditionInfos;

        public bool IsEnable => _isEnable;

        public IConditionInfo[] ConditionInfos => _conditionInfos != null ? _conditionInfos.Cast<IConditionInfo>().ToArray() : Array.Empty<IConditionInfo>();
    }
}