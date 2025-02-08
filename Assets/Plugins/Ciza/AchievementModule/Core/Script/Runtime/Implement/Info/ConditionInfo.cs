using System;
using UnityEngine;

namespace CizaAchievementModule.Implement
{
    [Serializable]
    public class ConditionInfo : IConditionInfo
    {
        [SerializeField]
        private bool _isEnable;

        [Space]
        [SerializeField]
        private string _statDataId;

        [SerializeField]
        private float _value;

        public bool IsEnable => _isEnable;

        public string StatDataId => _statDataId;

        public float Value => _value;
    }
}