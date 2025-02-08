using UnityEngine.Scripting;

namespace CizaSaveLoadModule
{
	public class SaveLoadModule
	{
		//private variable
		private readonly ISaveLoadModuleConfig _saveLoadModuleConfig;
		private readonly IIo                   _io;

		private readonly IWriterProvider _writerProvider;
		private readonly IReaderProvider _readerProvider;

		//public constructor
		[Preserve]
		public SaveLoadModule(ISaveLoadModuleConfig saveLoadModuleConfig, IIo io, IWriterProvider writerProvider, IReaderProvider readerProvider)
		{
			_saveLoadModuleConfig = saveLoadModuleConfig;
			_io                   = io;

			_writerProvider = writerProvider;
			_readerProvider = readerProvider;
		}

		//public method
		public void Save<T>(string key, T data, string filePath = null)
		{
			var fullPath = GetFullPath(filePath);
			_io.CreateDirectory(fullPath);
			
			var bufferSize = _saveLoadModuleConfig.BufferSize;
			var encoding   = _saveLoadModuleConfig.Encoding;
			var writer     = _writerProvider.CreateWriter(fullPath, bufferSize, encoding);

			writer.Write<T>(key, data);
			writer.Dispose();
		}

		public bool TryLoad<T>(string key, out T data, string filePath = null)
		{
			var fullPath = GetFullPath(filePath);
			if (!_io.CheckIsExist(fullPath))
			{
				data = default;
				return false;
			}

			var bufferSize = _saveLoadModuleConfig.BufferSize;
			var reader     = _readerProvider.CreateReader(fullPath, bufferSize);

			data = reader.Read<T>(key);
			reader.Dispose();

			return data != null;
		}

		//private method
		private string GetFullPath(string filePath)
		{
			var path                = string.IsNullOrWhiteSpace(filePath) ? _saveLoadModuleConfig.DefaultFilePath : filePath;
			var applicationDataPath = _saveLoadModuleConfig.ApplicationDataPath;
			var fullPath            = _io.GetFullPath(applicationDataPath, path);
			return fullPath;
		}
	}
}
