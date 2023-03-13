using NUnit.Framework;
using UnityEngine;


public class CameraModuleTest
{
    private CizaCameraModule.CameraModule _cameraModule;

    private Camera _camera;


    [SetUp]
    public void SetUp()
    {
        
        _cameraModule = new CizaCameraModule.CameraModule();

        var cameraGameObject = new GameObject();
        _camera = cameraGameObject.AddComponent<Camera>();
    }

    [TearDown]
    public void TearDown()
    {
        _cameraModule = null;
        _camera = null;
    }


    [Test]
    public void Should_Success_When_Register()
    {
        //arrange
        Assert.AreEqual(_cameraModule.Cameras.Count, 0);

        //act
        _cameraModule.Register("123", _camera);

        //Assert
        Assert.AreEqual(_cameraModule.Cameras.Count, 1);
    }


    [TestCase("123", 1000)]
    public void Should_Success_When_SetCameraData(string cameraName, int depth)
    {
        //arrange
        var layerMask = LayerMask.GetMask("UI");
        _cameraModule.Register(cameraName, _camera);
        Assert.AreEqual(_cameraModule.Cameras.Count, 1);

        //act
        _cameraModule.SetCameraData(cameraName, depth, layerMask);

        //Assert
        Assert.AreEqual(_camera.depth, depth);
        Assert.AreEqual(_camera.cullingMask, layerMask);
    }

    [TestCase("123")]
    public void Should_Success_When_OpenCamera(string cameraName)
    {
        //arrange
        _cameraModule.Register(cameraName, _camera);
        Assert.AreEqual(_cameraModule.Cameras.Count, 1);
        _camera.enabled = false;

        //act
        _cameraModule.OpenCamera(cameraName);

        //Assert
        Assert.AreEqual(_camera.enabled, true);
    }

    [TestCase("123")]
    public void Should_Success_When_CloseCamera(string cameraName)
    {
        //arrange
        _cameraModule.Register(cameraName, _camera);
        Assert.AreEqual(_cameraModule.Cameras.Count, 1);
        _camera.enabled = true;

        //act
        _cameraModule.CloseCamera(cameraName);

        //Assert
        Assert.AreEqual(_camera.enabled, false);
    }
}