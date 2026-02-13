using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Scripting;

namespace CizaTextModule
{
	public class TextMapLogic
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly Dictionary<string, TextModuleWithDataId> _textModuleMapByDataId;
		protected readonly string _className;

		protected readonly HashSet<ITextMap> _textMaps = new HashSet<ITextMap>();

		// EVENT: ---------------------------------------------------------------------------------

		// dataId, category
		public event Action<string, string> OnChangeCategory;

		// oriText
		public event Func<string, string> OnTranslate;

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual bool TryGetCategories(string dataId, out string[] categories)
		{
			if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
			{
				categories = Array.Empty<string>();
				return false;
			}

			categories = textModule.Categories;
			return true;
		}

		public virtual bool TryGetDefaultCategory(string dataId, out string defaultCategory)
		{
			if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
			{
				defaultCategory = string.Empty;
				return false;
			}

			defaultCategory = textModule.DefaultCategory;
			return true;
		}

		public virtual bool TryGetCurrentCategory(string dataId, out string currentCategory)
		{
			if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
			{
				currentCategory = string.Empty;
				return false;
			}

			currentCategory = textModule.CurrentCategory;
			return true;
		}

		public virtual bool TryGetText(string dataId, string key, out string text)
		{
			if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
			{
				text = string.Empty;
				return false;
			}

			return textModule.TryGetText(key, out text);
		}

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public TextMapLogic(TextModuleWithDataId[] textModules, string className)
		{
			_textModuleMapByDataId = new Dictionary<string, TextModuleWithDataId>();

			foreach (var textModule in textModules)
				_textModuleMapByDataId.Add(textModule.DataId, textModule);

			_className = className;

			foreach (var textModule in textModules)
			{
				textModule.OnChangeCategory += OnChangeCategoryImp;
				textModule.OnTranslate += OnTranslateImp;
			}
		}

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public virtual bool TryReset(string dataId)
		{
			if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
				return false;

			textModule.Reset();
			return true;
		}

		public virtual bool TryReloadDefaultTexts(string dataId)
		{
			if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
				return false;

			textModule.ReloadDefaultTexts();
			return true;
		}

		public virtual bool TryReloadDefaultTexts(string dataId, string defaultCategory)
		{
			if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
				return false;

			textModule.ReloadDefaultTexts(defaultCategory);
			return true;
		}

		public virtual bool TryReloadTexts(string dataId, string csv)
		{
			if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
				return false;

			textModule.ReloadTexts(csv);
			return true;
		}

		public virtual bool TryReloadTexts(string dataId, string csv, string defaultCategory)
		{
			if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
				return false;

			textModule.ReloadTexts(csv, defaultCategory);
			return true;
		}


		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual bool TryChangeCategory(string dataId, string category)
		{
			if (!_textModuleMapByDataId.TryGetValue(dataId, out var textModule))
				return false;

			if (textModule.TryChangeCategory(category))
			{
				RefreshAllTextMaps();
				OnChangeCategory?.Invoke(dataId, category);
				return true;
			}

			return false;
		}

		public virtual void RefreshAllTextMaps()
		{
			foreach (var textMap in _textMaps.ToArray())
				SetTextMap(textMap);
		}

		public virtual void AddTextMap(ITextMap textMap)
		{
			SetTextMap(textMap);
			_textMaps.Add(textMap);
		}

		public virtual void AddTextMaps(ITextMap[] textMaps)
		{
			foreach (var textMap in textMaps)
				AddTextMap(textMap);
		}

		public virtual void RemoveTextMap(ITextMap textMap) =>
			_textMaps.Remove(textMap);

		public virtual void RemoveTextMaps(ITextMap[] textMaps)
		{
			foreach (var textMap in textMaps)
				RemoveTextMap(textMap);
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual void OnChangeCategoryImp(string currentCategory)
		{
			foreach (var textMap in _textMaps)
				SetTextMap(textMap);
		}

		protected virtual void SetTextMap(ITextMap textMap)
		{
			if (!textMap.IsEnable)
				return;

			if (TrySetTextMapByKey(textMap))
				return;

			if (TrySetTextMapByKeyWithPattern(textMap))
				return;
		}


		protected virtual bool TrySetTextMapByKey(ITextMap textMap)
		{
			foreach (var textModule in _textModuleMapByDataId.Values.ToArray())
				if (textModule.TryGetText(textMap.Key, out var text))
				{
					textMap.SetText(text);
					return true;
				}

			return false;
		}

		protected virtual bool TrySetTextMapByKeyWithPattern(ITextMap textMap)
		{
			var text = textMap.Key;

			foreach (var textModule in _textModuleMapByDataId.Values.ToArray())
				if (textModule.TryGetTexts(text.GetKeys(textModule.KeyPattern), out var textMapByKey))
					text = text.Replace(textModule.KeyPattern, textMapByKey, _className, "TrySetTextMapByKeyWithPattern");

			if (!string.IsNullOrEmpty(text) && text != textMap.Key)
			{
				textMap.SetText(text);
				return true;
			}

			return false;
		}


		protected virtual string OnTranslateImp(string oriText) =>
			OnTranslate?.Invoke(oriText) ?? oriText;
	}
}