using CizaAudioModule;

public class FakeAudioData : IAudioData
{
    // public variable
    public string ClipDataId { get; }
    public string PrefabDataId { get; }
    public float SpatialBlend { get; }


    
    // public method
    public FakeAudioData(string clipDataId, string prefabDataId, float spatialBlend)
    {
        ClipDataId = clipDataId;
        PrefabDataId = prefabDataId;
        SpatialBlend = spatialBlend;
    }
}