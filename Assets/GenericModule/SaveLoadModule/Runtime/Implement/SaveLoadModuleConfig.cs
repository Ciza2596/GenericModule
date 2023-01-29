using System.IO;
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

        [SerializeField] private ReferenceModes _referenceMode;
        
        [Space]
        [SerializeField] private Directories _directory;
        [SerializeField] private string _path = "SaveLoadModuleFile.slv";

        [Space] [SerializeField] private int _bufferSize = 2048;

        public ReferenceModes ReferenceMode => _referenceMode;

        public string ApplicationDataPath
        {
            get
            {
                if (_directory == Directories.PersistentDataPath)
                    return Path.Combine(Application.persistentDataPath, _path);

                return Path.Combine(Application.dataPath, _path);
            }
        }

        public int BufferSize => _bufferSize;
        public Encoding Encoding => Encoding.UTF8;
    }
}