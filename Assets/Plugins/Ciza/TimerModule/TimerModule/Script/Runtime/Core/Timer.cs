using System;

namespace CizaTimerModule
{
	public class Timer : ITimerReadModel
	{
		private event Action<ITimerReadModel> _onComplete;
		private event Action<ITimerReadModel, float> _onTick;

		public void Initialize(string id, bool isOnce, float duration, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick)
		{
			Id = id;
			IsOnce = isOnce;
			Duration = duration;
			_onComplete = onComplete;
			_onTick = onTick;
		}

		public string Id { get; private set; }


		public bool IsOnce { get; private set; }

		public bool IsPlayed => Time >= Duration;
		public float Duration { get; private set; }
		public float Time { get; private set; }


		public void AddTime(float deltaTime) => Time += deltaTime;
		public void ResetTime() => Time = 0;

		public void OnComplete() => _onComplete?.Invoke(this);
		public void OnTick(float deltaTime) => _onTick?.Invoke(this, deltaTime);
	}
}