using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaAchievementModule
{
    [Serializable]
    public class Stat : IStat
    {
        public string DataId { get; }

        public float Min { get; }
        public float Max { get; }

        public float Current { get; private set; }

        [Preserve]
        public Stat(string dataId, float min, float max)
        {
            DataId = dataId;

            Min = min;
            Max = max;
        }

        public void SetCurrent(float value) =>
            Current = Mathf.Clamp(value, Min, Max);
    }
}