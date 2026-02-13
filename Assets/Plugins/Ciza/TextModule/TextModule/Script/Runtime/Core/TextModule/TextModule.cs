using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace CizaTextModule
{
	public class TextModule
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly ITextModuleConfig _config;
		protected Dictionary<string, Dictionary<string, string>> _textMapByCategoryByKey;

		// EVENT: ---------------------------------------------------------------------------------

		// Category
		public event Action<string> OnChangeCategory;

		// OriText, return New Text
		public event Func<string, string> OnTranslate;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual string[] Categories { get; protected set; }

		public virtual string DefaultCategory { get; protected set; }
		public virtual string CurrentCategory { get; protected set; }

		public virtual bool TryGetTexts(string[] keys, out Dictionary<string, string> textMapByKey)
		{
			textMapByKey = new Dictionary<string, string>();
			foreach (var key in keys)
				if (TryGetText(key, out var text))
					textMapByKey.Add(key, text);
			return textMapByKey.Count > 0;
		}

		public virtual bool TryGetText(string key, out string text)
		{
			if (!key.CheckHasValue())
			{
				text = string.Empty;
				return false;
			}

			if (!_textMapByCategoryByKey.TryGetValue(key, out var textMapByCategory))
			{
				text = string.Empty;
				return false;
			}

			if (!textMapByCategory.TryGetValue(CurrentCategory, out var oriText))
			{
				text = string.Empty;
				return false;
			}

			text = OnTranslate?.Invoke(oriText) ?? oriText;
			return true;
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public TextModule(ITextModuleConfig config)
		{
			_config = config;
			Reset();
		}

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public virtual void Reset()
		{
			Categories = Array.Empty<string>();
			DefaultCategory = string.Empty;
			CurrentCategory = string.Empty;
			_textMapByCategoryByKey = new Dictionary<string, Dictionary<string, string>>();
		}

		public virtual void ReloadDefaultTexts() =>
			ReloadDefaultTexts(false, string.Empty);

		public virtual void ReloadDefaultTexts(string defaultCategory) =>
			ReloadDefaultTexts(true, defaultCategory);

		public virtual void ReloadTexts(string csv) =>
			ReloadTexts(csv, false, string.Empty);

		public virtual void ReloadTexts(string csv, string defaultCategory) =>
			ReloadTexts(csv, true, defaultCategory);

		// PUBLIC METHOD: ----------------------------------------------------------------------


		public virtual void SetDefaultCategory() =>
			TryChangeCategory(DefaultCategory);

		public virtual bool TryChangeCategory(string category)
		{
			if (!Categories.Contains(category))
				return false;

			CurrentCategory = category;
			OnChangeCategory?.Invoke(CurrentCategory);
			return true;
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual void ReloadDefaultTexts(bool hasCustomDefaultCategory, string customDefaultCategory)
		{
			if (_config.TryGetCsv(out var csv))
				ReloadTexts(csv, hasCustomDefaultCategory, customDefaultCategory);
			else
				Reset();
		}

		protected virtual void ReloadTexts(string csv, bool hasCustomDefaultCategory, string customDefaultCategory)
		{
			Categories = CsvUtils.GetCategories(csv, "TextModule");
			Assert.IsTrue(Categories.Length > 0, "[TextModule::ReloadTexts] Categories length is zero. Please check textModuleConfig.");

			DefaultCategory = hasCustomDefaultCategory ? customDefaultCategory : Categories[0];
			Assert.IsTrue(Categories.Contains(DefaultCategory), "[TextModule::ReloadTexts] Categories is not include DefaultCategory. Please check textModuleConfig.");

			_textMapByCategoryByKey = CsvUtils.CreateTextMapByCategoryByKey(csv, "TextModule");
			Assert.IsNotNull(_textMapByCategoryByKey, "[TextModule::ReloadTexts] Reload texts fail. Please check textModuleConfig.");

			SetDefaultCategory();
		}
	}
}