using System;
using UnityEngine;

namespace CizaAchievementModule
{
    [Serializable]
    public class ExportedStat
    {
        [SerializeField]
        private string _dataId;

        [SerializeField]
        private float _current;

        public ExportedStat(string dataId, float current)
        {
            _dataId = dataId;
            _current = current;
        }

        public string DataId => _dataId;

        public float Current => _current;
    }
}