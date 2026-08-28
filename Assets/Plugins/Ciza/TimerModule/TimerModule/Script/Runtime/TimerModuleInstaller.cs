using System;

namespace CizaTimerModule
{
	public static class TimerModuleInstaller
	{
		public static TimerModule Install(bool isAutoInitialize = true) =>
			Install<TimerModule>(isAutoInitialize);

		public static TTimerModule Install<TTimerModule>(bool isAutoInitialize = true) where TTimerModule : TimerModule
		{
			var timerModule = Activator.CreateInstance(typeof(TTimerModule)) as TTimerModule;
			if (isAutoInitialize)
				timerModule.Initialize();
			return timerModule;
		}
	}
}