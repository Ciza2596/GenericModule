

using System;
using CizaObjectPool;
using NUnit.Framework;

public class ObjectPoolTest
{

    private ObjectPool<FakeObject> _fakeObjectPool; 

    
    [SetUp]
    public void SetUp()
    {
        _fakeObjectPool = new ObjectPool<FakeObject>();
    }


    [Test]
    public void Call_Spawn_Return_Obj()
    {
        // act
        var fakeObject = _fakeObjectPool.Spawn();
        
        // assert
        Assert.AreEqual(_fakeObjectPool.UsingCount,1);
        Assert.AreEqual(_fakeObjectPool.UnusingCount,0);
        Assert.IsNotNull(fakeObject, "Object cant be null");
    }

    [Test]
    public void Call_DeSpawn_Recycle_Obj()
    {

        // arrange
        var fakeObject = Spawn_Obj_And_Check();
        Assert.IsFalse(fakeObject.IsDispose);
        
        // act
        _fakeObjectPool.DeSpawn(fakeObject);
        
        // assert
        Assert.AreEqual(_fakeObjectPool.UsingCount,0);
        Assert.AreEqual(_fakeObjectPool.UnusingCount,1);
        Assert.IsTrue(fakeObject.IsDispose);
    }
    
    [Test]
    public void Call_Release_Clear_Pool()
    {

        // arrange
        var fakeObject = Spawn_Obj_And_Check();
        Assert.IsFalse(fakeObject.IsDispose);
        
        // act
        _fakeObjectPool.Release();
        
        // assert
        Assert.AreEqual(_fakeObjectPool.UsingCount,0);
        Assert.AreEqual(_fakeObjectPool.UnusingCount,0);
        Assert.IsTrue(fakeObject.IsDispose);
    }
    
    
    
    private FakeObject Spawn_Obj_And_Check()
    {
        var fakeObject = _fakeObjectPool.Spawn();
        
        Assert.AreEqual(_fakeObjectPool.UsingCount,1);
        Assert.AreEqual(_fakeObjectPool.UnusingCount,0);
        Assert.IsNotNull(fakeObject, "Object cant be null");

        return fakeObject;
    }
    
    private class FakeObject: IDisposable
    {
        public bool IsDispose { get; private set; }


        public void Dispose()
        {
            IsDispose = true;
        }
    }
}
