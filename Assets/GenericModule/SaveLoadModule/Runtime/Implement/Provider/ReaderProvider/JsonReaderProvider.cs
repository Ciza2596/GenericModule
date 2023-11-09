using System.IO;
using DataType;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	public class JsonReaderProvider : IReaderProvider
	{
		//private variable
		private readonly IStreamProvider     _streamProvider;
		private readonly IDataTypeController _dataTypeController;
		private readonly IReflectionHelper   _reflectionHelper;

		//public method
		[Preserve]
		public JsonReaderProvider(IStreamProvider streamProvider, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper)
		{
			_streamProvider     = streamProvider;
			_dataTypeController = dataTypeController;
			_reflectionHelper   = reflectionHelper;
		}

		public IReader CreateReader(string fullPath, int bufferSize)
		{
			var stream = _streamProvider.CreateStream(FileModes.Read, fullPath, bufferSize);
			Assert.IsNotNull(stream, $"[JsonReaderProvider::CreateReader] stream is null by fullPath: {fullPath}.");

			var reader = CreateReader(stream, bufferSize);
			return reader;
		}

		//private method
		private IReader CreateReader(Stream stream, int bufferSize) =>
			new JsonReader(stream, bufferSize, _dataTypeController, _reflectionHelper);
	}
}
