using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace CizaTextModule
{
	public class TextModule
	{
		private readonly ITextModuleConfig _textModuleConfig;

		private Dictionary<string, Dictionary<string, string>> _textMapByCategoryByKey;

		public event Action<string> OnChangeCategory;

		public string[] Categories { get; private set; }

		public string DefaultCategory { get; private set; }
		public string CurrentCategory { get; private set; }

		public TextModule(ITextModuleConfig textModuleConfig)
		{
			_textModuleConfig = textModuleConfig;

			ReloadTexts();
		}

		public void ReloadTexts() =>
			ReloadTexts(_textModuleConfig.CsvText);

		public void SetDefaultCategory() =>
			TryChangeCategory(DefaultCategory);

		public bool TryChangeCategory(string category)
		{
			if (!Categories.Contains(category))
				return false;

			CurrentCategory = category;
			OnChangeCategory?.Invoke(CurrentCategory);
			return true;
		}

		public bool TryGetText(string key, out string text)
		{
			if (!_textMapByCategoryByKey.TryGetValue(key, out var textMapByCategory))
			{
				text = string.Empty;
				if (_textModuleConfig.IsShowWarningLog)
					Debug.LogError($"[TextModule::TryGetText] Not find textRecordMapByCategory by key: {key}.");
				return false;
			}

			if (!textMapByCategory.TryGetValue(CurrentCategory, out text))
			{
				if (_textModuleConfig.IsShowWarningLog)
					Debug.LogError($"[TextModule::TryGetText] Not find textRecord by key: {key}.");
				return false;
			}

			return true;
		}

		private void ReloadTexts(string csvText)
		{
			Categories = CsvUtils.GetCategories(csvText, "TextModule");
			Assert.IsTrue(Categories.Length > 0, "[TextModule::ReloadTexts] Categories length is zero. Please check textModuleConfig.");

			DefaultCategory = _textModuleConfig.IsCustomDefaultCategory ? _textModuleConfig.CustomDefaultCategory : Categories[0];
			Assert.IsTrue(Categories.Contains(DefaultCategory), "[TextModule::ReloadTexts] Categories is not include DefaultCategory. Please check textModuleConfig.");

			_textMapByCategoryByKey = CsvUtils.CreateTextMapByCategoryByKey(csvText, "TextModule");
			Assert.IsNotNull(_textMapByCategoryByKey, "[TextModule::ReloadTexts] Reload texts fail. Please check textModuleConfig.");

			SetDefaultCategory();
		}
	}
}
