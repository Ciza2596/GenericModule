using UnityEngine;

namespace SaveLoadModule.Implement
{
    [CreateAssetMenu(fileName = "SaveLoadModule", menuName = "SaveLoadModule/FileStreamProviderConfig")]
    public class FileStreamProviderConfig : ScriptableObject, IFileStreamProviderConfig
    {
        private enum Directories
        {
            PersistentDataPath,
            DataPath
        }

        [SerializeField] private Directories _directory;
        [SerializeField] private string _path = "";

        [Space]
        [SerializeField] private int _bufferSize = 2028;

        public string Path
        {
            get
            {
                if (_directory == Directories.PersistentDataPath)
                    return IO.CombinePathAndFilename(Application.persistentDataPath, _path);

                return IO.CombinePathAndFilename(Application.dataPath, _path);
            }
        }

        public int BufferSize => _bufferSize;
    }
}