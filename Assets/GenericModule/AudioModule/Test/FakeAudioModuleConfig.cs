using CizaAudioModule;
using UnityEngine.Audio;

public class FakeAudioModuleConfig : IAudioModuleConfig
{
    public string PoolRootName { get; private set; } = "[AudioModule]";
    public string PoolPrefix { get; private set; } = "[";
    public string PoolSuffix { get; private set; } = "s]";

    public AudioMixer AudioMixer { get; private set; }

    public string MasterVolumeParameter { get; private set; }
    public string BgmVolumeParameter { get; private set; }
    public string SfxVolumeParameter { get; private set; }
    public string VoiceVolumeParameter { get; private set; }


    public void SetPoolRootName(string poolRootName) => PoolRootName = poolRootName;
    public void SetPoolPrefix(string poolPrefix) => PoolPrefix = poolPrefix;
    public void SetPoolSuffix(string poolSuffix) => PoolSuffix = poolSuffix;

    public void SetAudioMixer(AudioMixer audioMixer) => AudioMixer = audioMixer;

    public void SetMasterVolumeParameter(string masterVolumeParameter) => MasterVolumeParameter = masterVolumeParameter;
    public void SetBgmVolumeParameter(string bgmVolumeParameter) => BgmVolumeParameter = bgmVolumeParameter;
    public void SetSfxVolumeParameter(string sfxVolumeParameter) => SfxVolumeParameter = sfxVolumeParameter;
    public void SetVoiceVolumeParameter(string voiceVolumeParameter) => VoiceVolumeParameter = voiceVolumeParameter;
}