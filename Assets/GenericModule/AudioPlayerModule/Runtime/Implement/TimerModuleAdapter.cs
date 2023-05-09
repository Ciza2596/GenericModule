using System;
using CizaTimerModule;

namespace CizaAudioPlayerModule.Implement
{
	public class TimerModuleAdapter : ITimerModule
	{
		public TimerModuleAdapter(CizaTimerModule.TimerModule timerModule) => 
			_timerModule = timerModule;
		
		private readonly CizaTimerModule.TimerModule _timerModule;

		public string AddOnceTimer(float duration, Action onComplete) => _timerModule.AddLoopTimer(duration, timerReadModel => { onComplete?.Invoke(); });

		public void RemoveTimer(string id) => _timerModule.RemoveTimer(id);

		public void AddOnceTimer(float startValue, float endValue, float duration, Action<float> valueSetter, Action onComplete = null) =>
			_timerModule.AddOnceTimer(startValue, endValue, duration, (timerReadModel, value) => { valueSetter?.Invoke(value); }, timerReadModel => { onComplete?.Invoke(); });
	}
}
