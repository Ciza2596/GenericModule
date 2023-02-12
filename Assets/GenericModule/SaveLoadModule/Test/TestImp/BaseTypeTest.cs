using NUnit.Framework;
using SaveLoadModule.Implement;

public abstract class BaseTypeTest
{
    protected const string FILE_PATH = "TypeTest.slmf";
    protected const string SAVE_KEY = "KEY";

    protected SaveLoadModule.SaveLoadModule _saveLoadModule;
    protected FakeSaveLoadModuleConfig _saveLoadModuleConfig;

    private Io _io;


    [SetUp]
    public void SetUp()
    {
        _saveLoadModuleConfig = new FakeSaveLoadModuleConfig();

        var saveLoadModuleInstaller = new SaveLoadModuleInstaller();
        _saveLoadModule = saveLoadModuleInstaller.CreateSaveLoadModule(_saveLoadModuleConfig);

        _io = saveLoadModuleInstaller.GetIo();
    }

    [TearDown]
    public void TearDown()
    {
        var fullPath = _io.GetFullPath(_saveLoadModuleConfig.ApplicationDataPath, FILE_PATH);
        _io.DeleteFile(fullPath);
    }
}