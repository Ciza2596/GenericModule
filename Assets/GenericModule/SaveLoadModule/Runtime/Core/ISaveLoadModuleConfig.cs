namespace SaveLoadModule
{
    public interface ISaveLoadModuleConfig
    {
        public string ApplicationDataPath { get; }
        public int BufferSize { get; }
        System.Text.Encoding Encoding { get; }
    }
}