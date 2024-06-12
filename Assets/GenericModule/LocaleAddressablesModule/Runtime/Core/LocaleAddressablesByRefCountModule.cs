using System;
using System.Collections.Generic;
using System.Threading;
using CizaAddressablesModule;
using CizaLocalizationModule;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaLocaleAddressablesModule
{
    public class LocaleAddressablesByRefCountModule
    {
        private readonly AddressablesByRefCountModule _addressablesByRefCountModule;
        private readonly LocalizationModule _localizationModule;

        public event Func<string, UniTask> OnChangedLocaleBeforeAsync;
        public event Func<string, UniTask> OnChangedLocaleAsync;

        public event Func<string, UniTask> OnLoadAssetAsync;
        public event Action<string> OnUnloadAsset;

        public bool IsInitialized => _localizationModule.IsInitialized;

        public bool IsChangingLocale => _localizationModule.IsChangingLocale;

        public string DefaultLocale => _localizationModule.DefaultLocale;
        public string[] SupportLocales => _localizationModule.SupportLocales;
        public string CurrentLocale => _localizationModule.CurrentLocale;
        public string SourceLocale => _localizationModule.SourceLocale;

        public IReadOnlyDictionary<string, int> RefCountMapByAddress => _addressablesByRefCountModule.RefCountMapByAddress;

        [Preserve]
        public LocaleAddressablesByRefCountModule(string className, ILocaleAddressablesByRefCountModuleConfig localeAddressablesByRefCountModuleConfig)
        {
            _addressablesByRefCountModule = new AddressablesByRefCountModule(className);

            _addressablesByRefCountModule.OnLoadAssetAsync += OnLoadAssetAsyncImp;
            _addressablesByRefCountModule.OnUnloadAsset += OnUnloadAssetImp;

            _localizationModule = new LocalizationModule(className, localeAddressablesByRefCountModuleConfig);

            _localizationModule.OnChangedLocaleBeforeAsync += m_OnChangedLocaleBeforeAsync;
            _localizationModule.OnChangedLocaleAsync += m_OnChangedLocaleAsync;


            UniTask m_OnChangedLocaleBeforeAsync(string locale) =>
                OnChangedLocaleBeforeAsync?.Invoke(locale) ?? UniTask.CompletedTask;

            UniTask m_OnChangedLocaleAsync(string locale) =>
                OnChangedLocaleAsync?.Invoke(locale) ?? UniTask.CompletedTask;
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

        public UniTask ChangeToDefaultLocaleAsync()
        {
            if (!IsInitialized)
            {
                Debug.LogWarning($"[LocaleAddressablesByRefCountModule::ChangeToDefaultLocale] LocaleAddressablesByRefCountModule is not initialized.");
                return UniTask.CompletedTask;
            }

            return _localizationModule.ChangeToDefaultLocaleAsync();
        }

        public UniTask ChangeLocaleAsync(string locale)
        {
            if (!IsInitialized)
            {
                Debug.LogWarning($"[LocaleAddressablesByRefCountModule::ChangeLocale] LocaleAddressablesByRefCountModule is not initialized.");
                return UniTask.CompletedTask;
            }

            return _localizationModule.ChangeLocaleAsync(locale);
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

        private UniTask OnLoadAssetAsyncImp(string address)
        {
            if (OnLoadAssetAsync != null)
                return OnLoadAssetAsync.Invoke(address);

            return UniTask.CompletedTask;
        }

        private void OnUnloadAssetImp(string address) =>
            OnUnloadAsset?.Invoke(address);
    }
}