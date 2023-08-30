using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using CizaEventModule;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

public class EventModuleTest
{
    private interface IEvent { }
    private interface ISyncEvent: IEvent { }
    private interface IAsyncEvent: IEvent { }
    private class ImpEventModule: EventModule<IEvent,ISyncEvent,IAsyncEvent> { }

    private ImpEventModule _eventModule;

    [SetUp]
    public void SetUp()
    {
        _eventModule = new ImpEventModule();
    }


    private bool _isTestSyncActionTrigger;
    private struct TestSyncEvent : ISyncEvent
    {
        public bool IsTestSyncActionTrigger { get; }

        public TestSyncEvent(bool isTestSyncActionTrigger) =>
            IsTestSyncActionTrigger = isTestSyncActionTrigger;
        
    }

    private void OnTestSyncAction(TestSyncEvent @event) =>
        _isTestSyncActionTrigger = @event.IsTestSyncActionTrigger;


    [Test]
    public void _01_Should_HasOneSyncEventDelegateContainer_When_AddSyncListener()
    {
        //act
        _eventModule.AddSyncListener<TestSyncEvent>(OnTestSyncAction);


        //assert
        Assert.AreEqual(1, _eventModule.SyncEventCount,
                        "SyncEvents count not match AddSyncListener times. Please check AddSyncListener times.");
    }

    [Test]
    public void _02_Should_HasntSyncEventDelegateContainer_When_RemoveSyncListener()
    {
        //arrange
        _eventModule.AddSyncListener<TestSyncEvent>(OnTestSyncAction);
        Assert.AreEqual(1,_eventModule.SyncEventCount, 
                        "Test init fail. SyncEvents count not match AddSyncListener times. Please check AddSyncListener times.");


        //act
        _eventModule.RemoveSyncListener<TestSyncEvent>(OnTestSyncAction);

        //assert
        Assert.AreEqual(0,_eventModule.SyncEventCount, 
                        "SyncEvents count not match AddSyncListener times. Please check RemoveSyncListener times.");
    }

    [Test]
    public void _03_Should_isTestSyncActionTriggerBeTrue_When_NotifySyncEvent()
    {
        //arrange
        _isTestSyncActionTrigger = false;
        _eventModule.AddSyncListener<TestSyncEvent>(OnTestSyncAction);
        Assert.IsFalse(_isTestSyncActionTrigger,
                       "Test init fail. Please check _isTestSyncActionTrigger should be false.");
        Assert.AreEqual(1,_eventModule.SyncEventCount, 
                        "Test init fail. SyncEvents count not match AddSyncListener times. Please check AddSyncListener times.");


        //act
        _eventModule.NotifySyncEvent(new TestSyncEvent(true));

        //assert
        Assert.IsTrue(_isTestSyncActionTrigger, "NotifySyncEvent fail.");
    }


    private bool _isTestAsyncActionTrigger;
    private bool _hasCancellationToken;
    private struct TestAsyncEvent : IAsyncEvent
    {
        public bool IsTestAsyncActionTrigger { get; }

        public TestAsyncEvent(bool isTestAsyncActionTrigger) => IsTestAsyncActionTrigger = isTestAsyncActionTrigger;
    }

    private UniTask OnTestAsyncAction(TestAsyncEvent @event, CancellationToken cancellationToken)
    {
        _isTestAsyncActionTrigger = @event.IsTestAsyncActionTrigger;
        _hasCancellationToken = cancellationToken != default;
        return UniTask.CompletedTask;
    }


    [Test]
    public void _04_Should_HasOneAsyncEventDelegateContainer_When_AddAsyncListener()
    {
        //act
        _eventModule.AddAsyncListener<TestAsyncEvent>(OnTestAsyncAction);


        //assert
        Assert.AreEqual(1,_eventModule.AsyncEventCount, 
                        "AsyncEvents count not match AddAsyncListener times. Please check AddAsyncListener times.");
    }

    [Test]
    public void _05_Should_HasntAsyncEventDelegateContainer_When_RemoveAsyncListener()
    {
        //arrange
        _eventModule.AddAsyncListener<TestAsyncEvent>(OnTestAsyncAction);
        Assert.AreEqual(1,_eventModule.AsyncEventCount, 
                        "Test init fail. AsyncEvents count not match AddAsyncListener times. Please check AddAsyncListener times.");


        //act
        _eventModule.RemoveAsyncListener<TestAsyncEvent>(OnTestAsyncAction);

        //assert
        Assert.AreEqual(0,_eventModule.AsyncEventCount,
                        "AsyncEvents count not match AddAsyncListener times. Please check RemoveAsyncListener times.");
    }

    [Test]
    public async void _05_Should_IsTestAsyncActionTriggerBeTrue_And_HasCancellationTokenBeFalse_CancelToke_When_NotifyAsyncEvent()
    {
        //arrange
        _isTestAsyncActionTrigger = false;
        _eventModule.AddAsyncListener<TestAsyncEvent>(OnTestAsyncAction);
        Assert.IsFalse(_isTestAsyncActionTrigger,
                       "Test init fail. Please check _isTestAsyncActionTrigger should be false.");
        Assert.AreEqual(_eventModule.AsyncEventCount, 1,
                        "Test init fail. AsyncEvents count not match AddAsyncListener times. Please check AddAsyncListener times.");


        //act
        await _eventModule.NotifyAsyncEvent(new TestAsyncEvent(true));

        //assert
        Assert.IsTrue(_isTestAsyncActionTrigger, "NotifyAsyncEvent fail.");
        Assert.IsFalse(_hasCancellationToken, "NotifyAsyncEvent fail.");
    }
    
    [Test]
    public async void _06_Should_IsTestAsyncActionTriggerBeTrue_And_HasCancellationTokenBeTrue_When_NotifyAsyncEventWithCancellationTokenSource()
    {
        //arrange
        _isTestAsyncActionTrigger = false;
        _eventModule.AddAsyncListener<TestAsyncEvent>(OnTestAsyncAction);
        Assert.IsFalse(_isTestAsyncActionTrigger,
            "Test init fail. Please check _isTestAsyncActionTrigger should be false.");
        Assert.AreEqual(_eventModule.AsyncEventCount, 1,
            "Test init fail. AsyncEvents count not match AddAsyncListener times. Please check AddAsyncListener times.");


        //act
        await _eventModule.NotifyAsyncEvent(new TestAsyncEvent(true), out var cancellationTokenSource);

        //assert
        Assert.IsTrue(_isTestAsyncActionTrigger, "NotifyAsyncEvent fail.");
        Assert.IsTrue(_hasCancellationToken, "NotifyAsyncEvent fail.");
    }
}