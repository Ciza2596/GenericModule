using CizaLogModule;
using NUnit.Framework;

public class LogModuleTest
{
    private CizaLogModule.LogModule _logModule;
    private FakeLogModuleConfig _fakeLogModuleConfig;
    private FakeLogPrinter _fakeLogPrinter;

    private const string MESSAGE = "Hello";

    [SetUp]
    public void SetUp()
    {
        _fakeLogModuleConfig = new FakeLogModuleConfig();
        _fakeLogPrinter = new FakeLogPrinter();
        _logModule = new CizaLogModule.LogModule(_fakeLogModuleConfig, _fakeLogPrinter);
    }

    [TestCase(true, MESSAGE)]
    [TestCase(false, null)]
    public void _01_DeBug(bool isPrint, string resultMessage)
    {
        //arrange
        _fakeLogModuleConfig.SetIsPrintDebug(isPrint);
        
        
        //act
        _logModule.Debug(MESSAGE);
        

        //assert
        var message = _fakeLogPrinter.DebugMessage;
        Assert.AreEqual(resultMessage, message, "Message and ResultMessage doesnt match.");
    }

    [TestCase(true, MESSAGE)]
    [TestCase(false, null)]
    public void _02_Info(bool isPrint, string resultMessage)
    {
        //arrange
        _fakeLogModuleConfig.SetIsPrintInfo(isPrint);
        
        
        //act
        _logModule.Info(MESSAGE);
        

        //assert
        var message = _fakeLogPrinter.InfoMessage;
        Assert.AreEqual(resultMessage, message, "Message and ResultMessage doesnt match.");
    }

    [TestCase(true, MESSAGE)]
    [TestCase(false, null)]
    public void _03_Warn(bool isPrint, string resultMessage)
    {
        //arrange
        _fakeLogModuleConfig.SetIsPrintWarn(isPrint);
        
        
        //act
        _logModule.Warn(MESSAGE);
        

        //assert
        var message = _fakeLogPrinter.WarnMessage;
        Assert.AreEqual(resultMessage, message, "Message and ResultMessage doesnt match.");
    }

    [TestCase(true, MESSAGE)]
    [TestCase(false, null)]
    public void _04_Error(bool isPrint, string resultMessage)
    {
        //arrange
        _fakeLogModuleConfig.SetIsPrintError(isPrint);
        
        
        //act
        _logModule.Error(MESSAGE);
        

        //assert
        var message = _fakeLogPrinter.ErrorMessage;
        Assert.AreEqual(resultMessage, message, "Message and ResultMessage doesnt match.");
    }
}


public class FakeLogModuleConfig : ILogModuleConfig
{
    //public variable
    public bool IsPrintDebug { get; private set; }
    public bool IsPrintInfo { get; private set; }
    public bool IsPrintWarn { get; private set; }
    public bool IsPrintError { get; private set; }

    //public method
    public void SetIsPrintDebug(bool isPrint) => IsPrintDebug = isPrint;
    public void SetIsPrintInfo(bool isPrint) => IsPrintInfo = isPrint;
    public void SetIsPrintWarn(bool isPrint) => IsPrintWarn = isPrint;
    public void SetIsPrintError(bool isPrint) => IsPrintError = isPrint;
}


public class FakeLogPrinter : ILogPrinter
{
    //public variable
    public string DebugMessage { get; private set; }
    public string InfoMessage { get; private set; }
    public string WarnMessage { get; private set; }
    public string ErrorMessage { get; private set; }


    //public method
    public void PrintDebug(string message) => DebugMessage = message;

    public void PrintInfo(string message) => InfoMessage = message;

    public void PrintWarn(string message) => WarnMessage = message;

    public void PrintError(string message) => ErrorMessage = message;
}