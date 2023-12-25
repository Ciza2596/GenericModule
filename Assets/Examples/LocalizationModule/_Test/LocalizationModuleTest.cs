using CizaLocalizationModule;
using NSubstitute;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

public class LocalizationModuleTest
{
    public const string Tc = "tc";
    public const string En = "en";

    public static readonly string[] SupportLocales = new[] { Tc, En };
    public static readonly string SourceLocale = Tc;
    public static readonly string DefaultLocale = Tc;
    public static readonly char PrefixTag = '/';

    private LocalizationModule _localizationModule;
    private ILocalizationModuleConfig _localizationModuleConfig;


    [SetUp]
    public void SetUp()
    {
        _localizationModuleConfig = Substitute.For<ILocalizationModuleConfig>();
        SetConfig(SupportLocales, SourceLocale, DefaultLocale, PrefixTag);
        
        _localizationModule = new LocalizationModule(_localizationModuleConfig);
    }

    [Test]
    public void _01_Initialize()
    {
        // arrange
        Assert.IsFalse(_localizationModule.IsInitialized, "LocalizationModule should be not initialized.");

        // act
        _localizationModule.Initialize();

        // assert
        Assert.IsTrue(_localizationModule.IsInitialized, "LocalizationModule should be initialized.");
    }

    [Test]
    public void _02_Release()
    {
        // arrange
        _localizationModule.Initialize();
        Assert.IsTrue(_localizationModule.IsInitialized, "LocalizationModule should be initialized.");

        // act
        _localizationModule.Release();

        // assert
        Assert.IsFalse(_localizationModule.IsInitialized, "LocalizationModule should be not initialized.");
    }

    [Test]
    public async void _03_ChangeToDefaultLocale()
    {
        // arrange
        _01_Initialize();
        await _localizationModule.ChangeLocaleAsync(En);
        Assert.AreEqual(En, _localizationModule.CurrentLocale, $"LocalizationModule should be locale: {En}.");

        // act
        _localizationModule.ChangeToDefaultLocale();

        // assert
        Assert.AreEqual(DefaultLocale, _localizationModule.CurrentLocale, $"LocalizationModule should be locale: {DefaultLocale}.");
    }

    [Test]
    public async void _04_ChangeLocaleAsync()
    {
        // arrange
        _01_Initialize();
        Assert.AreEqual(DefaultLocale, _localizationModule.CurrentLocale, $"LocalizationModule should be locale: {DefaultLocale}.");

        // act
        await _localizationModule.ChangeLocaleAsync(En);

        // assert
        Assert.AreEqual(En, _localizationModule.CurrentLocale, $"LocalizationModule should be locale: {En}.");
    }

    private void SetConfig(string[] supportLocales, string sourceLocale, string defaultLocale, char prefixTag)
    {
        _localizationModuleConfig.SupportLocales.Returns(supportLocales);
        _localizationModuleConfig.SourceLocale.Returns(sourceLocale);
        _localizationModuleConfig.DefaultLocale.Returns(defaultLocale);
        _localizationModuleConfig.PrefixTag.Returns(prefixTag);
    }
}