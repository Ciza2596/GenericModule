using System;
using UnityEngine;

namespace CizaAchievementModule
{
    [Serializable]
    public class ExportedStat
    {
        [SerializeField]
        private string _dataId;

        [Space]
        [SerializeField]
        private float _current;

        [SerializeField]
        private bool _isUnlocked;

        public ExportedStat(string dataId, float current, bool isUnlocked)
        {
            _dataId = dataId;
            _current = current;
            _isUnlocked = isUnlocked;
        }

        public string DataId => _dataId;


        public float Current => _current;

        public bool IsUnlocked => _isUnlocked;
    }
}