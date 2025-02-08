using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule
{
    [Serializable]
    public class RumbleInfo : IRumbleInfo
    {
        [SerializeField]
        private string _dataId;

        [Space]
        [SerializeField]
        private int _order;
        
        [SerializeField]
        private float _duration;

        [Space]
        [SerializeField]
        private ControlSchemeInfo[] _controlSchemeInfos;


        public string DataId => _dataId;
        
        public int Order => _order;
        public float Duration => _duration;

        public bool TryGetControlSchemeInfo(string dataId, out IControlSchemeInfo controlSchemeInfo)
        {
            if (_controlSchemeInfos == null || _controlSchemeInfos.Length <= 0)
            {
                controlSchemeInfo = null;
                return false;
            }

            var chosenControlSchemeInfo = _controlSchemeInfos.FirstOrDefault(m_controlSchemeInfo => m_controlSchemeInfo.DataId == dataId);
            controlSchemeInfo = chosenControlSchemeInfo != null ? chosenControlSchemeInfo : _controlSchemeInfos[0];
            return true;
        }


        [Preserve]
        public RumbleInfo() { }

        [Preserve]
        public RumbleInfo(string dataId, float duration, ControlSchemeInfo[] controlSchemeInfos)
        {
            _dataId = dataId;
            _duration = duration;
            _controlSchemeInfos = controlSchemeInfos;
        }
    }
}