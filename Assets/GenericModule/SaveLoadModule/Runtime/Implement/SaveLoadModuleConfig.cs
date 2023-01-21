using System.IO;
using UnityEngine;

namespace SaveLoadModule.Implement
{
    [CreateAssetMenu(fileName = "SaveLoadModule", menuName = "SaveLoadModule/SaveLoadModuleConfig")]
    public class SaveLoadModuleConfig : ScriptableObject, ISaveLoadModuleConfig
    {
        private enum Directories
        {
            PersistentDataPath,
            DataPath
        }

        [SerializeField] private Directories _directory;
        [SerializeField] private string _path = "";

        [Space] [SerializeField] private int _bufferSize = 2028;

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
    }
}