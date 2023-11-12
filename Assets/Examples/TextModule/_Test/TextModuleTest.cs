using System.Linq;
using CizaTextModule;
using NUnit.Framework;

public class TextModuleTest
{
	private static readonly string[] Categories      = new[] { "Keyboard", "Xbox", "PS5" };
	private static readonly string   DefaultCategory = Categories[0];

	private static readonly string   Key   = "Confirm";
	private static readonly string[] Texts = new[] { "J_Icon", "A_Icon", "X_Icon" };

	private static readonly string TestCsvText = $"Key,{Categories[0]},{Categories[1]},{Categories[2]}\n {Key}, {Texts[0]}, {Texts[1]}, {Texts[2]}";

	[Test]
	public void _01_Check_DefaultCategory()
	{
		// act
		var textModule = CreateTextModule(TestCsvText);


		// assert
		Assert.AreEqual(DefaultCategory, textModule.DefaultCategory);
		Assert.AreEqual(DefaultCategory, textModule.CurrentCategory);
	}

	[Test]
	public void _02_Check_CustomDefaultCategory()
	{
		// act
		var textModule = CreateTextModule(Categories[1], TestCsvText);


		// assert
		Assert.AreEqual(Categories[1], textModule.DefaultCategory);
		Assert.AreEqual(Categories[1], textModule.CurrentCategory);
	}

	[Test]
	public void _03_Check_Categories()
	{
		// act
		var textModule = CreateTextModule(TestCsvText);


		// assert
		Assert.AreEqual(Categories.Length, textModule.Categories.Length);

		foreach (var category in textModule.Categories)
			Assert.IsTrue(Categories.Contains(category));
	}

	[Test]
	public void _04_TryChangeCategory()
	{
		// arrange
		var textModule = CreateTextModule(TestCsvText);
		Assert.AreEqual(DefaultCategory, textModule.DefaultCategory);

		textModule.TryGetText(Key, out var text1);
		Assert.AreEqual(Texts[0], text1);


		// act
		var changeSuccess = textModule.TryChangeCategory(Categories[2]);


		// assert
		Assert.IsTrue(changeSuccess);
		Assert.AreEqual(Categories[2], textModule.CurrentCategory);

		textModule.TryGetText(Key, out var text2);
		Assert.AreEqual(Texts[2], text2);
	}

	[Test]
	public void _05_TryGetText()
	{
		// arrange
		var textModule = CreateTextModule(TestCsvText);


		// act
		var hasText = textModule.TryGetText(Key, out var text);


		// assert
		Assert.IsTrue(hasText);
		Assert.AreEqual(Categories[0], textModule.CurrentCategory);
		Assert.AreEqual(Texts[0], text);
	}

	private TextModule CreateTextModule(string csvText) =>
		CreateTextModule(false, string.Empty, csvText);

	private TextModule CreateTextModule(string customDefaultCategory, string csvText) =>
		CreateTextModule(true, customDefaultCategory, csvText);

	private TextModule CreateTextModule(bool isCustomDefaultCategory, string customDefaultCategory, string csvText) =>
		new(new FakeTextModuleConfig(isCustomDefaultCategory, customDefaultCategory, csvText, false));

	private class FakeTextModuleConfig : ITextModuleConfig
	{
		public FakeTextModuleConfig(bool isCustomDefaultCategory, string customDefaultCategory, string csvText, bool isShowWarningLog)
		{
			IsCustomDefaultCategory = isCustomDefaultCategory;
			CustomDefaultCategory   = customDefaultCategory;

			CsvText          = csvText;
			IsShowWarningLog = isShowWarningLog;
		}

		public bool   IsCustomDefaultCategory { get; }
		public string CustomDefaultCategory   { get; }

		public string CsvText          { get; }
		public bool   IsShowWarningLog { get; }
	}
}
