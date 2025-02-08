using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule
{
    [Serializable]
    public class ControlSchemeInfo : IControlSchemeInfo
    {
        [SerializeField]
        private string _controlSchemeDataId;

        [Space]
        [SerializeField]
        private float _lowFrequency;

        [SerializeField]
        private float _highFrequency;

        public string DataId => _controlSchemeDataId;

        public float LowFrequency => _lowFrequency;
        public float HighFrequency => _highFrequency;

        [Preserve]
        public ControlSchemeInfo() { }

        [Preserve]
        public ControlSchemeInfo(string controlSchemeDataId, float lowFrequency, float highFrequency)
        {
            _controlSchemeDataId = controlSchemeDataId;
            _lowFrequency = lowFrequency;
            _highFrequency = highFrequency;
        }
    }
}