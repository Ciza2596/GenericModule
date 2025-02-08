using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAchievementModule
{
    [Serializable]
    public class ExportedStat
    {
        [SerializeField]
        private string _dataId;

        [SerializeField]
        private float _current;

        [Preserve]
        public ExportedStat() { }

        [Preserve]
        public ExportedStat(string dataId, float current)
        {
            _dataId = dataId;
            _current = current;
        }

        public string DataId => _dataId;

        public float Current => _current;
    }
}