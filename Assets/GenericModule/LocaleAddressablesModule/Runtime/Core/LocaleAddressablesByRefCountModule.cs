using System;
using System.Threading;
using CizaAddressablesModule;
using CizaLocalizationModule;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaLocaleAddressablesModule
{
	public class LocaleAddressablesByRefCountModule
	{
		private readonly AddressablesByRefCountModule _addressablesByRefCountModule = new AddressablesByRefCountModule();
		private readonly LocalizationModule           _localizationModule;

		public event Func<string, UniTask> OnChangedLocaleBefore;
		public event Func<string, UniTask> OnChangedLocale;

		public bool IsInitialized => _localizationModule.IsInitialized;

		public bool IsChangingLocale => _localizationModule.IsChangingLocale;

		public string   DefaultLocale  => _localizationModule.DefaultLocale;
		public string[] SupportLocales => _localizationModule.SupportLocales;
		public string   CurrentLocale  => _localizationModule.CurrentLocale;
		public string   SourceLocale   => _localizationModule.SourceLocale;

		public LocaleAddressablesByRefCountModule(ILocaleAddressablesByRefCountModuleConfig localeAddressablesByRefCountModuleConfig)
		{
			_localizationModule = new LocalizationModule(localeAddressablesByRefCountModuleConfig);

			_localizationModule.OnChangedLocaleBefore += m_OnChangedLocaleBefore;
			_localizationModule.OnChangedLocale       += m_OnChangedLocale;


			UniTask m_OnChangedLocaleBefore(string locale) =>
				OnChangedLocaleBefore?.Invoke(locale) ?? UniTask.CompletedTask;

			UniTask m_OnChangedLocale(string locale) =>
				OnChangedLocale?.Invoke(locale) ?? UniTask.CompletedTask;
		}

		public void Initialize()
		{
			if (IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::Initialize] LocaleAddressablesByRefCountModule is initialized.");
				return;
			}

			_localizationModule.Initialize();
		}

		public void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::Release] LocaleAddressablesByRefCountModule is not initialized.");
				return;
			}

			_localizationModule.Release();
		}

		#region Locale

		public void ChangeToDefaultLocale()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::ChangeToDefaultLocale] LocaleAddressablesByRefCountModule is not initialized.");
				return;
			}

			_localizationModule.ChangeToDefaultLocale();
		}

		public UniTask ChangeLocale(string locale)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::ChangeLocale] LocaleAddressablesByRefCountModule is not initialized.");
				return UniTask.CompletedTask;
			}

			return _localizationModule.ChangeLocale(locale);
		}

		#endregion

		#region Asset

		public UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken = default) where T : UnityEngine.Object
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::LoadAssetAsync] LocaleAddressablesByRefCountModule is not initialized.");
				return UniTask.FromResult<T>(null);
			}

			var addressWithLocalePrefix = _localizationModule.GetTextWithLocalePrefix(address);
			return _addressablesByRefCountModule.LoadAssetAsync<T>(addressWithLocalePrefix, cancellationToken);
		}

		public T GetAsset<T>(string address) where T : UnityEngine.Object
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::GetAsset] LocaleAddressablesByRefCountModule is not initialized.");
				return null;
			}

			var addressWithLocalePrefix = _localizationModule.GetTextWithLocalePrefix(address);
			return _addressablesByRefCountModule.GetAsset<T>(addressWithLocalePrefix);
		}

		public void UnloadAsset<T>(string address) where T : UnityEngine.Object
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::UnloadAsset] LocaleAddressablesByRefCountModule is not initialized.");
				return;
			}

			var addressWithLocalePrefix = _localizationModule.GetTextWithLocalePrefix(address);
			_addressablesByRefCountModule.UnloadAsset<T>(addressWithLocalePrefix);
		}

		public void UnloadAllAssets()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::UnloadAllAssets] LocaleAddressablesByRefCountModule is not initialized.");
				return;
			}

			_addressablesByRefCountModule.UnloadAllAssets();
		}

		#endregion
	}
}
