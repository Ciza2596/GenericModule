namespace CizaAudioModule
{
    public interface IBgmSettings
    {
        bool TryGetBgmInfo(out string bgmDataId, out float volume);
    }
}