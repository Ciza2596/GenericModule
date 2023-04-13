using CizaAudioPlayerModule;
using CizaAudioPlayerModule.Implement;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPlayerModuleExample : MonoBehaviour
{
    [SerializeField] private AudioDataOverview _audioDataOverview;
    [Space] [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private AudioPlayerModuleConfig _audioPlayerModuleConfig;
    [SerializeField] private AudioPlayerModuleAssetProvider _audioModulePlayerAssetProvider;

    private AudioPlayerModule _audioPlayerModule;

    private async void Awake()
    {
        _audioPlayerModule = new AudioPlayerModule(_audioPlayerModuleConfig, _audioModulePlayerAssetProvider, _audioMixer);

        var audioDataMap = _audioDataOverview.GetAudioDataMap();
        await _audioPlayerModule.Initialize(audioDataMap);

        var audioId = _audioPlayerModule.Play("BGMChannel1", "wind_bell");
        _audioPlayerModule.Stop(audioId);
    }

    private void Update() =>
        _audioPlayerModule.Tick(Time.deltaTime);
}