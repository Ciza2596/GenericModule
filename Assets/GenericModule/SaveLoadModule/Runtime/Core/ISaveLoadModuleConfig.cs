namespace SaveLoadModule
{
    public interface ISaveLoadModuleConfig
    {
        public string ApplicationDataPath { get; }

        public string DefaultFilePath { get; }

        public int BufferSize { get; }
        System.Text.Encoding Encoding { get; }
    }
}