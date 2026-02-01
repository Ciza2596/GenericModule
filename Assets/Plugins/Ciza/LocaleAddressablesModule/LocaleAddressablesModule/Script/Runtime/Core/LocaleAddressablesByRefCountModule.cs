using System;
using System.Collections.Generic;
using System.Threading;
using CizaAddressablesModule;
using CizaLocaleModule;
using CizaUniTask;
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

		public event Func<string, CancellationToken, UniTask> OnChangedLocaleBeforeAsync;
		public event Func<string, CancellationToken, UniTask> OnChangedLocaleAsync;

		public event Func<string, CancellationToken, UniTask> OnLoadAssetAsync;
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


			UniTask m_OnChangedLocaleBeforeAsync(string locale, CancellationToken cancellationToken) =>
				OnChangedLocaleBeforeAsync?.Invoke(locale, cancellationToken) ?? UniTask.CompletedTask;

			UniTask m_OnChangedLocaleAsync(string locale, CancellationToken cancellationToken) =>
				OnChangedLocaleAsync?.Invoke(locale, cancellationToken) ?? UniTask.CompletedTask;
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

		public virtual UniTask ChangeToDefaultLocaleAsync(CancellationToken cancellationToken)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::ChangeToDefaultLocale] LocaleAddressablesByRefCountModule is not initialized.");
				return UniTask.CompletedTask;
			}

			return _localeModule.ChangeToDefaultLocaleAsync(cancellationToken);
		}

		public virtual UniTask ChangeLocaleAsync(string locale, CancellationToken cancellationToken)
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::ChangeLocale] LocaleAddressablesByRefCountModule is not initialized.");
				return UniTask.CompletedTask;
			}

			return _localeModule.ChangeLocaleAsync(locale, cancellationToken);
		}

		#endregion

		#region Asset

		public virtual UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancellationToken) where T : Object
		{
			if (!IsInitialized)
			{
				Debug.LogWarning($"[LocaleAddressablesByRefCountModule::LoadAssetAsync] LocaleAddressablesByRefCountModule is not initialized.");
				return UniTask.FromResult<T>(null);
			}

			var addressWithLocalePrefix = _localeModule.GetTextWithLocalePrefix(address);
			return _addressablesByRefCountModule.LoadAssetAsync<T>(addressWithLocalePrefix, cancellationToken);
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

		protected virtual async UniTask OnLoadAssetAsyncImp(string address, CancellationToken cancellationToken)
		{
			if (OnLoadAssetAsync != null)
				await OnLoadAssetAsync.Invoke(address, cancellationToken);
		}

		protected virtual void OnUnloadAssetImp(string address) =>
			OnUnloadAsset?.Invoke(address);
	}
}