using System;
using System.Linq;
using CizaAsync;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaLocaleModule
{
	public class LocaleModule
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly string _className;
		protected readonly ILocaleModuleConfig _config;
		protected string[] _supportLocales;

		// EVENT: ---------------------------------------------------------------------------------

		public event Func<string, AsyncToken, Awaitable> OnChangedLocaleBeforeAsync;
		public event Func<string, AsyncToken, Awaitable> OnChangedLocaleAsync;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool IsInitialized { get; private set; }

		public virtual bool IsChangingLocale { get; private set; }

		public virtual string DefaultLocale { get; private set; }
		public virtual string[] SupportLocales => _supportLocales != null ? _supportLocales.ToArray() : Array.Empty<string>();
		public virtual string CurrentLocale { get; private set; }
		public virtual string SourceLocale { get; private set; }

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public LocaleModule(ILocaleModuleConfig config) : this("LocaleModule", config) { }

		[Preserve]
		public LocaleModule(string className, ILocaleModuleConfig config)
		{
			_className = className;
			_config = config;

			if (!_config.SupportLocales.Contains(_config.SourceLocale))
				Debug.LogError($"[{_className}::LocaleModule] SourceLocale: {_config.SourceLocale} is not in supportLocales.");

			if (!_config.SupportLocales.Contains(_config.DefaultLocale))
				Debug.LogError($"[{_className}::LocaleModule] DefaultLocale: {_config.DefaultLocale} is not in supportLocales.");
		}

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public virtual void Initialize()
		{
			if (IsInitialized)
			{
				Debug.LogWarning($"[{_className}::Initialize] LocaleModule is initialized.");
				return;
			}

			IsInitialized = true;

			_supportLocales = _config.SupportLocales;
			SourceLocale = _config.SourceLocale;
			DefaultLocale = _config.DefaultLocale;
			CurrentLocale = DefaultLocale;
		}

		public virtual void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[{_className}::Release] LocaleModule is not initialized.");
				return;
			}

			_supportLocales = null;
			SourceLocale = string.Empty;
			CurrentLocale = string.Empty;
			DefaultLocale = string.Empty;

			IsInitialized = false;
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual Awaitable ChangeToDefaultLocaleAsync(AsyncToken asyncToken) =>
			ChangeLocaleAsync(DefaultLocale, asyncToken);

		public virtual async Awaitable ChangeLocaleAsync(string locale, AsyncToken asyncToken)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[{_className}::ChangeLocale] LocaleModule is not initialized.");
				return;
			}

			if (!SupportLocales.Contains(locale))
			{
				Debug.LogError($"[{_className}::ChangeLocale] Locale: {locale} is not support.");
				return;
			}

			if (IsChangingLocale)
				return;

			IsChangingLocale = true;

			if (OnChangedLocaleBeforeAsync != null)
				await OnChangedLocaleBeforeAsync.Invoke(CurrentLocale, asyncToken);

			CurrentLocale = locale;

			if (OnChangedLocaleAsync != null)
				await OnChangedLocaleAsync.Invoke(CurrentLocale, asyncToken);

			IsChangingLocale = false;
		}

		public virtual string GetTextWithLocalePrefix(string text)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[{_className}::GetTextWithLocalePrefix] LocaleModule is not initialized.");
				return string.Empty;
			}

			var localePrefix = _config.IsIgnoreSourceLocale && CurrentLocale.Equals(SourceLocale) ? string.Empty : CurrentLocale + _config.PrefixTag;
			var textByAddLocalePrefix = localePrefix + text;

			return textByAddLocalePrefix;
		}
	}
}