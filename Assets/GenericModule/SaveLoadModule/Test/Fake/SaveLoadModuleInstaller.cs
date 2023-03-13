using DataType.Implement;
using CizaSaveLoadModule;
using CizaSaveLoadModule.Implement;

public class SaveLoadModuleInstaller
{

    private Io _io;
    
    public CizaSaveLoadModule.SaveLoadModule CreateSaveLoadModule(ISaveLoadModuleConfig saveLoadModuleConfig)
    {
        var fakeReflectionHelperConfig = new FakeReflectionHelperConfig();
        var reflectionHelper = new ReflectionHelper(fakeReflectionHelperConfig);
        
        _io = new Io();

        var dataTypeController = new DataTypeControllerAdapter(reflectionHelper);

        var fileStreamProvider = new FileStreamProvider();
        var jsonWriterProvider = new JsonWriterProvider(fileStreamProvider, dataTypeController, reflectionHelper);
        var jsonReaderProvider = new JsonReaderProvider(fileStreamProvider, dataTypeController, reflectionHelper);

        return new CizaSaveLoadModule.SaveLoadModule(saveLoadModuleConfig, _io, jsonWriterProvider, jsonReaderProvider);
    }

    public Io GetIo() => _io;
}