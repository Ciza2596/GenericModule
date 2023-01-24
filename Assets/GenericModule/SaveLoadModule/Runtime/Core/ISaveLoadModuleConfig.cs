namespace SaveLoadModule
{
    public interface ISaveLoadModuleConfig
    {

        public ReferenceModes ReferenceMode { get; }
        public string ApplicationDataPath { get; }
        
        public int BufferSize { get; }

    }
}