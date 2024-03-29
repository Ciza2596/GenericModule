using System;
using System.Linq;
using UnityEngine;

namespace CizaAudioModule.Implement
{
    [CreateAssetMenu(fileName = "BgmControllerConfig", menuName = "Ciza/AudioModule/BgmControllerConfig", order = 200)]
    public class BgmControllerConfig : ScriptableObject, IBgmControllerConfig
    {
        [SerializeField]
        private float _fadeTime = 0.5f;

        [Space]
        [SerializeField]
        private string[] _bgmDataIds;

        public float FadeTime => _fadeTime;

        public string[] BgmDataIds => _bgmDataIds != null ? _bgmDataIds.ToHashSet().ToArray() : Array.Empty<string>();
    }
}