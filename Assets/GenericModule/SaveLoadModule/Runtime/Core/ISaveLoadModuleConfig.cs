using System.Text;

namespace SaveLoadModule
{
    public interface ISaveLoadModuleConfig
    {
        public string ApplicationDataPath { get; }
        public string DefaultFilePath { get; }

        public int BufferSize { get; }
        public Encoding Encoding { get; }
    }
}