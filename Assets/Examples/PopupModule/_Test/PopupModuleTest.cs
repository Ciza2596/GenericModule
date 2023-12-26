using CizaPopupModule;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;

public class PopupModuleTest
{
    public const string ResourcePath = "PopupModule/";
    public const string PopupCanvas = "TestPopupCanvas";
    public const string Popup = "TestPopup";

    public const string RootName = "[PopupModule]";

    public const string Key = "Default_Key";
    public const string DataId = "Default_DataId";
    public const bool IsAutoHideWhenConfirm = true;

    public const string ContentTip = "ContentTip";
    public const string ConfirmTip = "ConfirmTip";
    public const string CancelTip = "CancelTip";

    private PopupModule _popupModule;
    private IPopupModuleConfig _popupModuleConfig;


    [SetUp]
    public void SetUp()
    {
        _popupModuleConfig = Substitute.For<IPopupModuleConfig>();
        SetConfig();

        _popupModule = new PopupModule(_popupModuleConfig);
    }

    [Test]
    public void _01_Initialize()
    {
        // arrange
        Assert.IsFalse(_popupModule.IsInitialized, "PopupModule should not be initialized.");

        // act
        _popupModule.Initialize();

        // assert
        Assert.IsTrue(_popupModule.IsInitialized, "PopupModule should be initialized.");
    }

    [Test]
    public void _02_Release()
    {
        // arrange
        _popupModule.Initialize();
        Assert.IsTrue(_popupModule.IsInitialized, "PopupModule should be initialized.");

        // act
        _popupModule.Release();

        // assert
        Assert.IsFalse(_popupModule.IsInitialized, "PopupModule should not be initialized.");
    }

    [Test]
    public void _03_Create_Popup_With_Hasnt_Cancel()
    {
        // arrange
        _01_Initialize();
        Assert.IsFalse(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should not be created.");

        // act
        _popupModule.CreatePopup(Key, DataId, ContentTip, ConfirmTip);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(popupReadModel.State, PopupStates.Invisible, $"Popup Key: {Key} should be invisible.");
    }

    [Test]
    public void _04_Create_Popup_With_Has_Cancel()
    {
        // arrange
        _01_Initialize();
        Assert.IsFalse(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should not be created.");

        // act
        _popupModule.CreatePopup(Key, DataId, IsAutoHideWhenConfirm, ContentTip, ConfirmTip, CancelTip);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(popupReadModel.State, PopupStates.Invisible, $"Popup Key: {Key} should be invisible.");
    }

    [Test]
    public void _05_DestroyPopup()
    {
        // arrange
        _03_Create_Popup_With_Hasnt_Cancel();

        // act
        _popupModule.DestroyPopup(Key);

        // act
        Assert.IsFalse(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should not be created.");
    }

    [Test]
    public void _06_DestroyAllPopups()
    {
        // arrange
        _03_Create_Popup_With_Hasnt_Cancel();

        // act
        _popupModule.DestroyAllPopups();

        // act
        Assert.IsFalse(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should not be created.");
    }

    [Test]
    public void _07_ShowImmediately()
    {
        // arrange
        _04_Create_Popup_With_Has_Cancel();

        // act
        _popupModule.ShowImmediately(Key);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupStates.Visible, popupReadModel.State, $"Popup Key: {Key} should be visible.");
    }

    [Test]
    public async void _08_ShowAsync()
    {
        // arrange
        _04_Create_Popup_With_Has_Cancel();

        // act
        await _popupModule.ShowAsync(Key);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupStates.Visible, popupReadModel.State, $"Popup Key: {Key} should be visible.");
    }

    [Test]
    public void _09_HideImmediately()
    {
        // arrange
        _07_ShowImmediately();

        // act
        _popupModule.HideImmediately(Key);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupStates.Invisible, popupReadModel.State, $"Popup Key: {Key} should be invisible.");
    }

    [Test]
    public async void _10_HideAsync()
    {
        // arrange
        _07_ShowImmediately();

        // act
        await _popupModule.HideAsync(Key);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupStates.Invisible, popupReadModel.State, $"Popup Key: {Key} should be invisible.");
    }

    [Test]
    public void _11_Select()
    {
        // arrange
        _07_ShowImmediately();

        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupModule.CancelIndex, popupReadModel.Index, $"Popup Key: {Key} should be index: {PopupModule.CancelIndex}.");

        // act
        _popupModule.Select(Key, PopupModule.ConfrimIndex);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupModule.ConfrimIndex, popupReadModel.Index, $"Popup Key: {Key} should be index: {PopupModule.ConfrimIndex}.");
    }

    [Test]
    public void _12_MoveToPrevious()
    {
        // arrange
        _07_ShowImmediately();

        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupModule.CancelIndex, popupReadModel.Index, $"Popup Key: {Key} should be index: {PopupModule.CancelIndex}.");

        // act
        _popupModule.MoveToPrevious(Key);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupModule.ConfrimIndex, popupReadModel.Index, $"Popup Key: {Key} should be index: {PopupModule.ConfrimIndex}.");
    }

    [Test]
    public void _13_MoveToNext()
    {
        // arrange
        _07_ShowImmediately();

        _popupModule.Select(Key, PopupModule.ConfrimIndex);
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupModule.ConfrimIndex, popupReadModel.Index, $"Popup Key: {Key} should be index: {PopupModule.ConfrimIndex}.");

        // act
        _popupModule.MoveToNext(Key);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupModule.CancelIndex, popupReadModel.Index, $"Popup Key: {Key} should be index: {PopupModule.CancelIndex}.");
    }

    [Test]
    public async void _14_ConfirmAsync_When_Select_Confirm()
    {
        // arrange
        _07_ShowImmediately();

        _popupModule.Select(Key, PopupModule.ConfrimIndex);
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupModule.ConfrimIndex, popupReadModel.Index, $"Popup Key: {Key} should be index: {PopupModule.ConfrimIndex}.");
        Assert.IsFalse(popupReadModel.HasConfirm, $"Popup Key: {Key} should not be confirm.");

        // act
        await _popupModule.ConfirmAsync(Key);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.IsTrue(popupReadModel.HasConfirm, $"Popup Key: {Key} should be confirm.");
    }

    [Test]
    public async void _15_ConfirmAsync_When_Select_Cancel()
    {
        // arrange
        _07_ShowImmediately();

        _popupModule.Select(Key, PopupModule.CancelIndex);
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out var popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.AreEqual(PopupModule.CancelIndex, popupReadModel.Index, $"Popup Key: {Key} should be index: {PopupModule.CancelIndex}.");
        Assert.IsFalse(popupReadModel.HasConfirm, $"Popup Key: {Key} should not be confirm.");

        // act
        await _popupModule.ConfirmAsync(Key);

        // act
        Assert.IsTrue(_popupModule.TryGetPopupReadModel(Key, out popupReadModel), $"Popup Key: {Key} should be created.");
        Assert.IsTrue(popupReadModel.HasConfirm, $"Popup Key: {Key} should be confirm.");
    }

    private void SetConfig()
    {
        _popupModuleConfig.RootName.Returns(RootName);
        _popupModuleConfig.IsDontDestroyOnLoad.Returns(false);

        _popupModuleConfig.CanvasPrefab.Returns(GetCanvasPrefab());
        _popupModuleConfig.TryGetPopupPrefab(DataId, out var popupPrefab).Returns(x =>
        {
            x[1] = GetPopupPrefab();
            return true;
        });
    }


    private GameObject GetCanvasPrefab() =>
        GetResourcesGameObject(PopupCanvas);

    private GameObject GetPopupPrefab() =>
        GetResourcesGameObject(Popup);

    private GameObject GetResourcesGameObject(string dataId) =>
        Resources.Load<GameObject>(ResourcePath + dataId);
}