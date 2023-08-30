using CizaAudioModule;

public class FakeAudioData : IAudioData
{
    public string ClipDataId { get; }
    public string PrefabDataId { get; }
    public float SpatialBlend { get; }

    public FakeAudioData(string clipDataId, string prefabDataId, float spatialBlend)
    {
        ClipDataId = clipDataId;
        PrefabDataId = prefabDataId;
        SpatialBlend = spatialBlend;
    }
}