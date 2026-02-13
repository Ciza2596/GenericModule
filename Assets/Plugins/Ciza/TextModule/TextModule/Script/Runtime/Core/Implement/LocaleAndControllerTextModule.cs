using System;
using UnityEngine.Scripting;

namespace CizaTextModule
{
	public class LocaleAndControllerTextModule
	{
		// CONST & STATIC: -----------------------------------------------------------------------

		public const string LOCALE_TEXT_MODULE_DATA_ID = "LocaleTextModule";
		public const string CONTROLLER_TEXT_MODULE_DATA_ID = "ControllerTextModule";

		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly TextMapLogic _textMapLogic;


		// EVENT: ---------------------------------------------------------------------------------

		public event Action<string> OnChangeLocaleCategory;
		public event Action<string> OnChangeControllerCategory;

		// OriText, return New Text
		public event Func<string, string> OnTranslate;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		#region Local

		public virtual string[] LocaleCategories =>
			_textMapLogic.TryGetCategories(LOCALE_TEXT_MODULE_DATA_ID, out var categories) ? categories : Array.Empty<string>();

		public virtual string LocaleDefaultCategory =>
			_textMapLogic.TryGetDefaultCategory(LOCALE_TEXT_MODULE_DATA_ID, out var defaultCategory) ? defaultCategory : string.Empty;

		public string LocaleCurrentCategory =>
			_textMapLogic.TryGetCurrentCategory(LOCALE_TEXT_MODULE_DATA_ID, out var currentCategory) ? currentCategory : string.Empty;

		public virtual bool TryGetLocaleText(string localeTextKey, out string localeText) =>
			_textMapLogic.TryGetText(LOCALE_TEXT_MODULE_DATA_ID, localeTextKey, out localeText);

		#endregion


		#region Controller

		public virtual string[] ControllerCategories =>
			_textMapLogic.TryGetCategories(CONTROLLER_TEXT_MODULE_DATA_ID, out var categories) ? categories : Array.Empty<string>();

		public virtual string ControllerDefaultCategory =>
			_textMapLogic.TryGetDefaultCategory(CONTROLLER_TEXT_MODULE_DATA_ID, out var defaultCategory) ? defaultCategory : string.Empty;

		public virtual string ControllerCurrentCategory =>
			_textMapLogic.TryGetCurrentCategory(CONTROLLER_TEXT_MODULE_DATA_ID, out var currentCategory) ? currentCategory : string.Empty;

		public virtual bool TryGetControllerText(string controllerTextKey, out string controllerText) =>
			_textMapLogic.TryGetText(CONTROLLER_TEXT_MODULE_DATA_ID, controllerTextKey, out controllerText);

		#endregion

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public LocaleAndControllerTextModule() : this(new TextModuleConfigInput(), new TextModuleConfigInput()) { }

		[Preserve]
		public LocaleAndControllerTextModule(ITextModuleConfig localeTextModuleConfig, ITextModuleConfig controllerTextModuleConfig)
		{
			_textMapLogic = new TextMapLogic(new TextModuleWithDataId[] { new TextModuleWithDataId(LOCALE_TEXT_MODULE_DATA_ID, StringExtension.LOCALE_TEXT_KEY_PATTERN, localeTextModuleConfig), new TextModuleWithDataId(CONTROLLER_TEXT_MODULE_DATA_ID, StringExtension.CONTROLLER_TEXT_KEY_PATTERN, controllerTextModuleConfig), }, "LocaleAndControllerTextModule");

			_textMapLogic.OnChangeCategory += OnChangeCategoryImp;
			_textMapLogic.OnTranslate += OnTranslateImp;
		}

		[Preserve]
		public LocaleAndControllerTextModule(TextModuleWithDataId localeTextModule, TextModuleWithDataId controllerTextModule)
		{
			_textMapLogic = new TextMapLogic(new TextModuleWithDataId[] { localeTextModule, controllerTextModule, }, "LocaleAndControllerTextModule");

			_textMapLogic.OnChangeCategory += OnChangeCategoryImp;
			_textMapLogic.OnTranslate += OnTranslateImp;
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual bool TryReset() =>
			TryResetLocale() && TryResetController();

		public virtual bool TryReloadDefaultTexts() =>
			TryReloadLocaleDefaultTexts() && TryReloadControllerDefaultTexts();

		public virtual bool TryReloadDefaultTexts(string defaultCategory) =>
			TryReloadLocaleDefaultTexts(defaultCategory) && TryReloadControllerDefaultTexts(defaultCategory);

		public virtual bool TryReloadTexts(string localeCsvText, string controllerCsvText, string defaultCategory) =>
			TryReloadLocaleTexts(localeCsvText, defaultCategory) && TryReloadControllerTexts(controllerCsvText, defaultCategory);


		#region Locale

		public virtual bool TryResetLocale() =>
			_textMapLogic.TryReset(LOCALE_TEXT_MODULE_DATA_ID);

		public virtual bool TryReloadLocaleDefaultTexts() =>
			_textMapLogic.TryReloadDefaultTexts(LOCALE_TEXT_MODULE_DATA_ID);

		public virtual bool TryReloadLocaleDefaultTexts(string defaultCategory) =>
			_textMapLogic.TryReloadDefaultTexts(LOCALE_TEXT_MODULE_DATA_ID, defaultCategory);

		public virtual bool TryReloadLocaleTexts(string csvText) =>
			_textMapLogic.TryReloadTexts(LOCALE_TEXT_MODULE_DATA_ID, csvText);

		public virtual bool TryReloadLocaleTexts(string csvText, string defaultCategory) =>
			_textMapLogic.TryReloadTexts(LOCALE_TEXT_MODULE_DATA_ID, csvText, defaultCategory);

		public virtual bool TryChangeLocaleCategory(string category) =>
			_textMapLogic.TryChangeCategory(LOCALE_TEXT_MODULE_DATA_ID, category);

		#endregion

		#region Controller

		public virtual bool TryResetController() =>
			_textMapLogic.TryReset(CONTROLLER_TEXT_MODULE_DATA_ID);

		public virtual bool TryReloadControllerDefaultTexts() =>
			_textMapLogic.TryReloadDefaultTexts(CONTROLLER_TEXT_MODULE_DATA_ID);

		public virtual bool TryReloadControllerDefaultTexts(string defaultCategory) =>
			_textMapLogic.TryReloadDefaultTexts(CONTROLLER_TEXT_MODULE_DATA_ID, defaultCategory);

		public virtual bool TryReloadControllerTexts(string csvText) =>
			_textMapLogic.TryReloadTexts(CONTROLLER_TEXT_MODULE_DATA_ID, csvText);

		public virtual bool TryReloadControllerTexts(string csvText, string defaultCategory) =>
			_textMapLogic.TryReloadTexts(CONTROLLER_TEXT_MODULE_DATA_ID, csvText, defaultCategory);

		public virtual bool TryChangeControllerCategory(string category) =>
			_textMapLogic.TryChangeCategory(CONTROLLER_TEXT_MODULE_DATA_ID, category);

		#endregion

		#region TextMap

		public virtual void RefreshAllTextMaps() =>
			_textMapLogic.RefreshAllTextMaps();

		public virtual void AddTextMap(ITextMap textMap) =>
			_textMapLogic.AddTextMap(textMap);

		public virtual void RemoveTextMap(ITextMap textMap) =>
			_textMapLogic.RemoveTextMap(textMap);

		public virtual void AddTextMaps(ITextMap[] textMaps) =>
			_textMapLogic.AddTextMaps(textMaps);

		public virtual void RemoveTextMaps(ITextMap[] textMaps) =>
			_textMapLogic.RemoveTextMaps(textMaps);

		#endregion

		// PROTECT METHOD: --------------------------------------------------------------------

		protected virtual void OnChangeCategoryImp(string dataId, string category)
		{
			if (dataId == LOCALE_TEXT_MODULE_DATA_ID)
				OnChangeLocaleCategory?.Invoke(category);

			else if (dataId == CONTROLLER_TEXT_MODULE_DATA_ID)
				OnChangeControllerCategory?.Invoke(category);
		}

		protected virtual string OnTranslateImp(string oriText) =>
			OnTranslate?.Invoke(oriText) ?? oriText;
	}
}