using System.IO;
using DataType;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	[Preserve]
	public class JsonWriterProvider : IWriterProvider
	{
		//private variable
		private readonly IStreamProvider     _streamProvider;
		private readonly IDataTypeController _dataTypeController;
		private readonly IReflectionHelper   _reflectionHelper;

		//public method
		[Preserve]
		public JsonWriterProvider(IStreamProvider streamProvider, IDataTypeController dataTypeController, IReflectionHelper reflectionHelper)
		{
			_streamProvider     = streamProvider;
			_dataTypeController = dataTypeController;
			_reflectionHelper   = reflectionHelper;
		}

		public IWriter CreateWriter(string fullPath, int bufferSize, System.Text.Encoding encoding)
		{
			var stream = _streamProvider.CreateStream(FileModes.Write, fullPath, bufferSize);
			Assert.IsNotNull(stream, "[JsonWriteProvider::CreateWriter] Create stream is fail.");

			var writer = CreateWrite(stream, encoding);
			return writer;
		}

		//private method
		private IWriter CreateWrite(Stream stream, System.Text.Encoding encoding) =>
			new JsonWriter(stream, encoding, _dataTypeController, _reflectionHelper);
	}
}
