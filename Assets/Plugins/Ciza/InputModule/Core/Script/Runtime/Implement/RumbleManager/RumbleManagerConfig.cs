using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CizaInputModule.Implement
{
    [CreateAssetMenu(fileName = "RumbleManagerConfig", menuName = "Ciza/InputModule/RumbleManagerConfig")]
    public class RumbleManagerConfig : ScriptableObject, IRumbleManagerConfig
    {
        [SerializeField]
        private RumbleInfo[] _rumbleInfos;

        public string[] AllDataIds
        {
            get
            {
                var allDataIds = new HashSet<string>();
                var rumbleInfos = _rumbleInfos != null ? _rumbleInfos : Array.Empty<RumbleInfo>();
                foreach (var rumbleInfo in rumbleInfos)
                    if (!string.IsNullOrEmpty(rumbleInfo.DataId))
                        allDataIds.Add(rumbleInfo.DataId);

                return allDataIds.ToArray();
            }
        }


        public bool TryGetRumbleInfo(string dataId, out IRumbleInfo rumbleInfo)
        {
            if (_rumbleInfos == null)
            {
                rumbleInfo = null;
                return false;
            }

            rumbleInfo = _rumbleInfos.FirstOrDefault(rumbleInfo => rumbleInfo.DataId == dataId);
            return rumbleInfo != null;
        }
    }
}