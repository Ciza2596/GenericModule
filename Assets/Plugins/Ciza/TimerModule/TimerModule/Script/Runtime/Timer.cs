using System;

namespace CizaTimerModule
{
	public class Timer : ITimerReadModel
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected event Action<ITimerReadModel> _onComplete;
		protected event Action<ITimerReadModel, float> _onTick;


		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		public string Id { get; private set; }

		public virtual bool IsOnce { get; private set; }

		public virtual bool IsPlayed => Time >= Duration;
		public virtual float Duration { get; protected set; }
		public virtual float Time { get; protected set; }

		// LIFECYCLE METHOD: ------------------------------------------------------------------

		public virtual void Initialize(string id, bool isOnce, float duration, float time, Action<ITimerReadModel> onComplete, Action<ITimerReadModel, float> onTick, params object[] args)
		{
			Id = id;
			IsOnce = isOnce;
			Duration = duration;
			Time = time;
			_onComplete = onComplete;
			_onTick = onTick;
			OnTick(0);
		}


		// PUBLIC METHOD: ----------------------------------------------------------------------

		public virtual void AddTime(float deltaTime) => Time += deltaTime;
		public virtual void ResetTime() => Time = 0;

		public virtual void OnComplete() => _onComplete?.Invoke(this);
		public virtual void OnTick(float deltaTime) => _onTick?.Invoke(this, deltaTime);
	}
}