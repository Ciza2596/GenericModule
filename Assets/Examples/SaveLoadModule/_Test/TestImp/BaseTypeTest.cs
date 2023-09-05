using NUnit.Framework;
using CizaSaveLoadModule.Implement;

public abstract class BaseTypeTest
{
    protected const string _filePath = "TypeTest.slmf";
    protected const string _saveKey = "KEY";

    protected CizaSaveLoadModule.SaveLoadModule _saveLoadModule;
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
        var fullPath = _io.GetFullPath(_saveLoadModuleConfig.ApplicationDataPath, _filePath);
        _io.DeleteFile(fullPath);
    }
}