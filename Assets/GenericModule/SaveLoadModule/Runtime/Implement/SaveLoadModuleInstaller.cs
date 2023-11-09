using DataType.Implement;
using UnityEngine.Scripting;

namespace CizaSaveLoadModule.Implement
{
	[Preserve]
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
