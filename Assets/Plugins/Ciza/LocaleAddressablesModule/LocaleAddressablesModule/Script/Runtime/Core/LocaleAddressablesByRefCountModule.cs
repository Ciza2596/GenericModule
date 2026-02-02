using System;
using System.Collections.Generic;
using CizaAddressablesModule;
using CizaLocaleModule;
using CizaAsync;
using UnityEngine;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace CizaLocaleAddressablesModule
{
	public class LocaleAddressablesByRefCountModule
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly AddressablesByRefCountModule _addressablesByRefCountModule;
		protected readonly LocaleModule _localeModule;

		// EVENT: ---------------------------------------------------------------------------------

		public event Func<string, AsyncToken, Awaitable> OnChangedLocaleBeforeAsync;
		public event Func<string, AsyncToken, Awaitable> OnChangedLocaleAsync;

		public event Func<string, AsyncToken, Awaitable> OnLoadAssetAsync;
		public event Action<string> OnUnloadAsset;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public virtual bool IsInitialized => _localeModule.IsInitialized;

		public virtual bool IsChangingLocale => _localeModule.IsChangingLocale;

		public virtual string DefaultLocale => _localeModule.DefaultLocale;
		public virtual string[] SupportLocales => _localeModule.SupportLocales;
		public virtual string CurrentLocale => _localeModule.CurrentLocale;
		public virtual string SourceLocale => _localeModule.SourceLocale;

		public virtual IReadOnlyDictionary<string, int> RefCountMapByAddress => _addressablesByRefCountModule.RefCountMapByAddress;

		// CONSTRUCTOR: ------------------------------------------------------------------------

		[Preserve]
		public LocaleAddressablesByRefCountModule(string className, ILocaleAddressablesByRefCountModuleConfig localeAddressablesByRefCountModuleConfig)
		{
			_addressablesByRefCountModule = new AddressablesByRefCountModule(className);

			_addressablesByRefCountModule.OnLoadAssetAsync += OnLoadAssetAsyncImp;
			_addressablesByRefCountModule.OnUnloadAsset += OnUnloadAssetImp;

			_localeModule = new LocaleModule(className, localeAddressablesByRefCountModuleConfig);

			_localeModule.OnChangedLocaleBeforeAsync += m_OnChangedLocaleBeforeAsync;
			_localeModule.OnChangedLocaleAsync += m_OnChangedLocaleAsync;


			Awaitable m_OnChangedLocaleBeforeAsync(string locale, AsyncToken asyncToken) =>
				OnChangedLocaleBeforeAsync?.Invoke(locale, asyncToken) ?? Async.Completed;

			Awaitable m_OnChangedLocaleAsync(string locale, AsyncToken asyncToken) =>
				OnChangedLocaleAsync?.Invoke(locale, asyncToken) ?? Async.Completed;
		}

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public virtual void Initialize()
		{
			if (IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::Initialize] LocaleAddressablesByRefCountModule is initialized.");
				return;
			}

			_localeModule.Initialize();
		}

		public virtual void Release()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::Release] LocaleAddressablesByRefCountModule is not initialized.");
				return;
			}

			_localeModule.Release();
		}

		// PUBLIC METHOD: ----------------------------------------------------------------------

		#region Locale

		public virtual Awaitable ChangeToDefaultLocaleAsync(AsyncToken asyncToken)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::ChangeToDefaultLocale] LocaleAddressablesByRefCountModule is not initialized.");
				return Async.Completed;
			}

			return _localeModule.ChangeToDefaultLocaleAsync(asyncToken);
		}

		public virtual Awaitable ChangeLocaleAsync(string locale, AsyncToken asyncToken)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::ChangeLocale] LocaleAddressablesByRefCountModule is not initialized.");
				return Async.Completed;
			}

			return _localeModule.ChangeLocaleAsync(locale, asyncToken);
		}

		#endregion

		#region Asset

		public virtual Awaitable<T> LoadAssetAsync<T>(string address, AsyncToken asyncToken) where T : Object
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::LoadAssetAsync] LocaleAddressablesByRefCountModule is not initialized.");
				return GetNullAwaitable<T>();
			}

			var addressWithLocalePrefix = _localeModule.GetTextWithLocalePrefix(address);
			return _addressablesByRefCountModule.LoadAssetAsync<T>(addressWithLocalePrefix, asyncToken);
		}

		public virtual T GetAsset<T>(string address) where T : Object
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::GetAsset] LocaleAddressablesByRefCountModule is not initialized.");
				return null;
			}

			var addressWithLocalePrefix = _localeModule.GetTextWithLocalePrefix(address);
			return _addressablesByRefCountModule.GetAsset<T>(addressWithLocalePrefix);
		}

		public virtual void UnloadAsset<T>(string address) where T : Object
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::UnloadAsset] LocaleAddressablesByRefCountModule is not initialized.");
				return;
			}

			var addressWithLocalePrefix = _localeModule.GetTextWithLocalePrefix(address);
			_addressablesByRefCountModule.UnloadAsset<T>(addressWithLocalePrefix);
		}

		public virtual void UnloadAllAssets()
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::UnloadAllAssets] LocaleAddressablesByRefCountModule is not initialized.");
				return;
			}

			_addressablesByRefCountModule.UnloadAllAssets();
		}

		#endregion

		protected virtual async Awaitable OnLoadAssetAsyncImp(string address, AsyncToken asyncToken)
		{
			if (OnLoadAssetAsync != null)
				await OnLoadAssetAsync.Invoke(address, asyncToken);
		}

		protected virtual void OnUnloadAssetImp(string address) =>
			OnUnloadAsset?.Invoke(address);

		private async Awaitable<T> GetNullAwaitable<T>() where T : Object
		{
			await Awaitable.MainThreadAsync();
			return null;
		}
	}
}