using System;

namespace CizaLocaleModule
{
	public static class LocaleModuleInstaller
	{
		public static LocaleModule Install(string className, ILocaleModuleConfig config, bool isAutoInitialize = true) =>
			Install<LocaleModule>(className, config, isAutoInitialize);

		public static TLocaleModule Install<TLocaleModule>(string className, ILocaleModuleConfig config, bool isAutoInitialize = true) where TLocaleModule : LocaleModule
		{
			var localeModule = Activator.CreateInstance(typeof(TLocaleModule), className, config) as TLocaleModule;
			if (isAutoInitialize)
				localeModule.Initialize();
			return localeModule;
		}
	}
}