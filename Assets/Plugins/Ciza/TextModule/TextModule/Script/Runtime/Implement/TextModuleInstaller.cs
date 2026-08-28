using System;

namespace CizaTextModule
{
	public static class TextModuleInstaller
	{
		public static TLocaleAndControllerTextModule Install<TLocaleAndControllerTextModule>(ITextModuleConfig localeTextModuleConfig, ITextModuleConfig controllerTextModuleConfig, bool isAutoInitialize = true) where TLocaleAndControllerTextModule : LocaleAndControllerTextModule
		{
			var localeAndControllerTextModule = Activator.CreateInstance(typeof(TLocaleAndControllerTextModule), localeTextModuleConfig, controllerTextModuleConfig) as TLocaleAndControllerTextModule;
			if (isAutoInitialize)
				localeAndControllerTextModule.TryReloadDefaultTexts();
			return localeAndControllerTextModule;
		}
	}
}