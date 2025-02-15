using System;
using UnityEngine;

namespace CizaAudioModule.Implement
{
    [Serializable]
    public class BgmSettings : IBgmSettings
    {
        [SerializeField]
        private bool _isEnable = true;

        [SerializeField]
        private string _bgmDataId;

        [Range(0, 1)]
        [SerializeField]
        private float _volume = 1;


        public bool TryGetBgmInfo(out string bgmDataId, out float volume)
        {
            volume = _volume;
            return TryGetAudioDataId(_bgmDataId, out bgmDataId);
        }

        private bool TryGetAudioDataId(string inputBgmDataId, out string outputBgmDataId)
        {
            if (!inputBgmDataId.CheckHasValue() || !_isEnable)
            {
                outputBgmDataId = string.Empty;
                return false;
            }

            outputBgmDataId = inputBgmDataId;
            return true;
        }
    }
}