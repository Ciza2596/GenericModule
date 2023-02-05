using System.Text;
using UnityEngine;

namespace SaveLoadModule.Implement
{
    [CreateAssetMenu(fileName = "SaveLoadModuleConfig", menuName = "SaveLoadModule/SaveLoadModuleConfig")]
    public class SaveLoadModuleConfig : ScriptableObject, ISaveLoadModuleConfig
    {
        private enum Directories
        {
            PersistentDataPath,
            DataPath
        }

        [Space] [SerializeField] private Directories _directory;
        [SerializeField] private string _defaultFilePath = "SaveLoadModuleFile.slmf";

        [Space] [SerializeField] private int _bufferSize = 2048;

        public string ApplicationDataPath
        {
            get
            {
                if (_directory == Directories.PersistentDataPath)
                    return Application.persistentDataPath;

                return Application.dataPath;
            }
        }

        public string DefaultFilePath => _defaultFilePath;

        public int BufferSize => _bufferSize;
        public Encoding Encoding => Encoding.UTF8;
    }
}