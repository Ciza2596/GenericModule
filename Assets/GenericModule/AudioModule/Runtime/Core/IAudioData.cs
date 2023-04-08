namespace CizaAudioModule
{
    public interface IAudioData
    {
        public string ClipDataId { get; }
        public string PrefabDataId { get; }
        public float SpatialBlend { get; }
    }
}