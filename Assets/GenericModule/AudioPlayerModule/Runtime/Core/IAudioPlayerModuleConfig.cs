using CizaAudioModule;

namespace CizaAudioPlayerModule
{
    public interface IAudioPlayerModuleConfig : IAudioModuleConfig
    {
        public float DefaultFadeTime { get; }
    }
}