using System;
using UnityEngine;

namespace CizaAchievementModule.Implement
{
    [Serializable]
    public class StatInfo : IStatInfo
    {
        [SerializeField]
        private string _dataId;
        
        [Space]
        [SerializeField]
        private float _min = 0;

        [SerializeField]
        private float _max = float.MaxValue;

        public string DataId => _dataId;
        
        public float Min => _min;
        public float Max => _max;
    }
}