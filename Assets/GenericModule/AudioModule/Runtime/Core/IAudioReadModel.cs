
namespace CizaAudioModule
{
    public interface IAudioReadModel : IAudioData
    {
        public string Id { get; }
        public float Volume { get; }
    }
}
