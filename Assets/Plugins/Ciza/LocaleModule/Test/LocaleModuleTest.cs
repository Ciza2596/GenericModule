using System.Threading;
using System.Threading.Tasks;
using CizaAsync;
using CizaLocaleModule;
using NSubstitute;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

public class LocaleModuleTest
{
	public const string Text = "Text";

	public const string Tc = "tc";
	public const string En = "en";

	public static readonly string[] SupportLocales = new[] { Tc, En };
	public static readonly string SourceLocale = Tc;
	public static readonly string DefaultLocale = Tc;
	public static readonly char PrefixTag = '/';

	private LocaleModule _localeModule;
	private ILocaleModuleConfig _localeModuleConfig;


	[SetUp]
	public void SetUp()
	{
		_localeModuleConfig = Substitute.For<ILocaleModuleConfig>();
		SetConfig(SupportLocales, SourceLocale, DefaultLocale, PrefixTag);

		_localeModule = new LocaleModule(_localeModuleConfig);
	}

	[Test]
	public void _01_Initialize()
	{
		// arrange
		Assert.IsFalse(_localeModule.IsInitialized, "LocaleModule should be not initialized.");

		// act
		_localeModule.Initialize();

		// assert
		Assert.IsTrue(_localeModule.IsInitialized, "LocaleModule should be initialized.");
	}

	[Test]
	public void _02_Release()
	{
		// arrange
		_localeModule.Initialize();
		Assert.IsTrue(_localeModule.IsInitialized, "LocaleModule should be initialized.");

		// act
		_localeModule.Release();

		// assert
		Assert.IsFalse(_localeModule.IsInitialized, "LocaleModule should be not initialized.");
	}

	[Test]
	public async Task _03_ChangeToDefaultLocale()
	{
		// arrange
		_01_Initialize();
		await _localeModule.ChangeLocaleAsync(En, AsyncToken.NONE);
		Assert.AreEqual(En, _localeModule.CurrentLocale, $"LocaleModule should be locale: {En}.");

		// act
		await _localeModule.ChangeToDefaultLocaleAsync(AsyncToken.NONE);

		// assert
		Assert.AreEqual(DefaultLocale, _localeModule.CurrentLocale, $"LocaleModule should be locale: {DefaultLocale}.");
	}

	[Test]
	public async Task _04_ChangeLocaleAsync()
	{
		// arrange
		_01_Initialize();
		Assert.AreEqual(DefaultLocale, _localeModule.CurrentLocale, $"LocaleModule should be locale: {DefaultLocale}.");

		// act
		await _localeModule.ChangeLocaleAsync(En, AsyncToken.NONE);

		// assert
		Assert.AreEqual(En, _localeModule.CurrentLocale, $"LocaleModule should be locale: {En}.");
	}

	[Test]
	public void _05_GetTextWithLocalePrefix_When_isIgnoreSourceLocale_True()
	{
		// arrange
		_01_Initialize();
		Assert.AreEqual(DefaultLocale, _localeModule.CurrentLocale, $"LocaleModule should be locale: {DefaultLocale}.");
		_localeModuleConfig.IsIgnoreSourceLocale.Returns(true);

		// act
		var text = _localeModule.GetTextWithLocalePrefix(Text);

		// assert
		Assert.AreEqual(Text, text, $"LocaleModule should be locale: {Text}.");
	}

	[Test]
	public void _06_GetTextWithLocalePrefix_When_isIgnoreSourceLocale_False()
	{
		// arrange
		_01_Initialize();
		Assert.AreEqual(DefaultLocale, _localeModule.CurrentLocale, $"LocaleModule should be locale: {DefaultLocale}.");
		_localeModuleConfig.IsIgnoreSourceLocale.Returns(false);

		// act
		var text = _localeModule.GetTextWithLocalePrefix(Text);

		// assert
		var expectedText = DefaultLocale + PrefixTag + Text;
		Assert.AreEqual(expectedText, text, $"LocaleModule should be locale: {expectedText}.");
	}

	private void SetConfig(string[] supportLocales, string sourceLocale, string defaultLocale, char prefixTag)
	{
		_localeModuleConfig.SupportLocales.Returns(supportLocales);
		_localeModuleConfig.SourceLocale.Returns(sourceLocale);
		_localeModuleConfig.DefaultLocale.Returns(defaultLocale);
		_localeModuleConfig.PrefixTag.Returns(prefixTag);
	}
}