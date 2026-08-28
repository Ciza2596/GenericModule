using System;

namespace CizaLocaleAddressablesModule.Implement
{
	public static class LocaleAddressablesModuleInstaller
	{
		public static LocaleAddressablesByRefCountModule Install(string className, ILocaleAddressablesByRefCountModuleConfig config, bool isAutoInitialize = true) =>
			Install<LocaleAddressablesByRefCountModule>(className, config, isAutoInitialize);

		public static TLocaleAddressablesModule Install<TLocaleAddressablesModule>(string className, ILocaleAddressablesByRefCountModuleConfig config, bool isAutoInitialize = true) where TLocaleAddressablesModule : LocaleAddressablesByRefCountModule
		{
			var localeAddressablesModule = Activator.CreateInstance(typeof(TLocaleAddressablesModule), className, config) as TLocaleAddressablesModule;
			if (isAutoInitialize)
				localeAddressablesModule.Initialize();
			return localeAddressablesModule;
		}
	}
}