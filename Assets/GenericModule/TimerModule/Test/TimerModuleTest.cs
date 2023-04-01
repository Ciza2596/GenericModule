using CizaTimerModule;
using NUnit.Framework;

public class TimerModuleTest
{
    private const string SAY_HELLO = "Hello";
    private const float TRIGGER_TIME = 2f;


    private TimerModule _timerModule;

    [SetUp]
    public void SetUp()
    {
        _timerModule = new TimerModule();
    }

    [TearDown]
    public void TearDown()
    {
        _timerModule = null;
    }


    [Test]
    public void _01_IsInitialized_Be_True_By_Initialize()
    {
        // arrange
        Check_TimerModule_Is_Initialized(false);

        // act
        _timerModule.Initialize();

        // assert
        Check_TimerModule_Is_Initialized(true);
    }


    [Test]
    public void _02_IsInitialized_Be_False_By_Release()
    {
        // arrange
        _timerModule.Initialize();
        Check_TimerModule_Is_Initialized(true);

        // act
        _timerModule.Release();

        // assert
        Check_TimerModule_Is_Initialized(false);
    }

    [Test]
    public void _03_Timer_Add_DeltaTime_By_Tick()
    {
        // arrange
        Initialize_And_Check_TimerModule();

        var timerId = _timerModule.AddLoopTimer(TRIGGER_TIME, null);

        // act
        _timerModule.Tick(1);

        // assert
        Check_Timer_Time(timerId, 1);
    }

    [Test]
    public void _04_Add_Once_Timer_By_AddOnceTimer()
    {
        // arrange
        Initialize_And_Check_TimerModule();

        // act
        var timerId = _timerModule.AddOnceTimer(TRIGGER_TIME, null);

        // assert
        Check_Timer_Is_Exist(timerId, true);
        Check_Timer_Is_Once(timerId, true);
        Check_Timer_TriggerTime(timerId, TRIGGER_TIME);
    }

    [Test]
    public void _05_Add_Loop_Timer_By_AddLoopTimer()
    {
        // arrange
        Initialize_And_Check_TimerModule();

        // act
        var timerId = _timerModule.AddLoopTimer(TRIGGER_TIME, null);

        // assert
        Check_Timer_Is_Exist(timerId, true);
        Check_Timer_Is_Once(timerId, false);
        Check_Timer_TriggerTime(timerId, TRIGGER_TIME);
    }


    [Test]
    public void _06_Remove_Timer_By_RemoveTimer()
    {
        // arrange
        Initialize_And_Check_TimerModule();
        var timerId = _timerModule.AddOnceTimer(TRIGGER_TIME, null);
        Check_Timer_Is_Exist(timerId, true);
        
        // act
        _timerModule.RemoveTimer(timerId);
        
        // assert
        Check_Timer_Is_Exist(timerId, false);
    }

    [Test]
    public void _07_Once_Timer_Auto_Is_removed_After_Action_Trigger()
    {
        // arrange
        Initialize_And_Check_TimerModule();

        var timerId = _timerModule.AddOnceTimer(TRIGGER_TIME, SayHello);
        var sayHello = string.Empty;

        // act
        _timerModule.Tick(TRIGGER_TIME);

        // assert
        Assert.AreEqual(SAY_HELLO, sayHello, "Action is not trigger.");
        Check_Timer_Is_Exist(timerId, false);

        void SayHello() =>
            sayHello = SAY_HELLO;
    }

    [Test]
    public void _08_Loop_Timer_Is_Not_Removed_After_Action_Is_Trigger()
    {
        // arrange
        Initialize_And_Check_TimerModule();

        var timerId = _timerModule.AddLoopTimer(TRIGGER_TIME, SayHello);
        var sayHello = string.Empty;

        // act
        _timerModule.Tick(TRIGGER_TIME);

        // assert
        Assert.AreEqual(SAY_HELLO, sayHello, "Action is not trigger.");
        Check_Timer_Is_Exist(timerId, true);

        void SayHello() =>
            sayHello = SAY_HELLO;
    }

    private void Check_TimerModule_Is_Initialized(bool expectedIsInitialized) =>
        Assert.AreEqual(expectedIsInitialized, _timerModule.IsInitialized);

    private void Initialize_And_Check_TimerModule()
    {
        _timerModule.Initialize();
        Check_TimerModule_Is_Initialized(true);
    }

    private void Check_Timer_Is_Exist(string timerId, bool expectedIsExist)
    {
        var isExist = _timerModule.TryGetTimerReadModel(timerId, out var timerReadModel);
        Assert.AreEqual(expectedIsExist, isExist);
    }

    private void Check_Timer_Is_Once(string timerId, bool expectedIsOnce)
    {
        var isExist = _timerModule.TryGetTimerReadModel(timerId, out var timerReadModel);
        Assert.IsTrue(isExist, $"Timer is not exist. timerId: {timerId}.");
        Assert.AreEqual(expectedIsOnce, timerReadModel.IsOnce);
    }

    private void Check_Timer_TriggerTime(string timerId, float expectedTriggerTime)
    {
        var isExist = _timerModule.TryGetTimerReadModel(timerId, out var timerReadModel);
        Assert.IsTrue(isExist, $"Timer is not exist. timerId: {timerId}.");
        Assert.AreEqual(expectedTriggerTime, timerReadModel.TriggerTime);
    }

    private void Check_Timer_Time(string timerId, float expectedTime)
    {
        var isExist = _timerModule.TryGetTimerReadModel(timerId, out var timerReadModel);
        Assert.IsTrue(isExist, $"Timer is not exist. timerId: {timerId}.");
        Assert.AreEqual(expectedTime, timerReadModel.Time);
    }
}