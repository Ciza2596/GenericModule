

namespace SaveLoadModule.Implement
{
    public interface IFileStreamProviderConfig
    {
        string Path { get; }
        int BufferSize { get; }
    }
}
