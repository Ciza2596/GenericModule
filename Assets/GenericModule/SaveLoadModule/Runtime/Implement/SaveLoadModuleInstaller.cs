using DataType.Implement;

namespace CizaSaveLoadModule.Implement
{
	public class SaveLoadModuleInstaller
	{
		public static SaveLoadModule Install(ISaveLoadModuleConfig saveLoadModuleConfig, IReflectionHelperConfig reflectionHelperConfig)
		{
			var fileStreamProvider        = new FileStreamProvider();
			var reflectionHelper          = new ReflectionHelper(reflectionHelperConfig);
			var dataTypeControllerAdapter = new DataTypeControllerAdapter(reflectionHelper);

			var jsonWriterProvider = new JsonWriterProvider(fileStreamProvider, dataTypeControllerAdapter, reflectionHelper);
			var jsonReaderProvider = new JsonReaderProvider(fileStreamProvider, dataTypeControllerAdapter, reflectionHelper);

			return new SaveLoadModule(saveLoadModuleConfig, new Io(), jsonWriterProvider, jsonReaderProvider);
		}
	}
}
