using System.Text;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	[CreateAssetMenu(fileName = "SaveLoadModuleConfig", menuName = "Ciza/SaveLoadModule/SaveLoadModuleConfig")]
	[Preserve]
	public class SaveLoadModuleConfig : ScriptableObject, ISaveLoadModuleConfig
	{
		public enum Directories
		{
			PersistentDataPath,
			DataPath
		}

		// VARIABLE: -----------------------------------------------------------------------------

		[Space]
		[SerializeField]
		protected Directories _directory;

		[SerializeField]
		protected string _defaultFilePath;

		[Space]
		[SerializeField]
		protected int _bufferSize;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string ApplicationDataPath
		{
			get
			{
				if (_directory == Directories.PersistentDataPath)
					return Application.persistentDataPath;

				return Application.dataPath;
			}
		}

		public virtual string DefaultFilePath => _defaultFilePath;

		public virtual int BufferSize => _bufferSize;
		public virtual Encoding Encoding => Encoding.UTF8;


		// CONSTRUCTOR: ------------------------------------------------------------------------

		public virtual void Reset()
		{
			_directory = Directories.PersistentDataPath;
			_defaultFilePath = "SaveLoadModuleFile.slmf";
			_bufferSize = 2048;
		}
	}
}