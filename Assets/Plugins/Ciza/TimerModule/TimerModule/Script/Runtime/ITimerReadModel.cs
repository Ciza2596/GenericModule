using UnityEngine;

namespace CizaTimerModule
{
	public interface ITimerReadModel
	{
		string Id { get; }

		bool IsOnce { get; }

		bool IsPlayed { get; }
		float Duration { get; }
		float Time { get; }
		float Normalized => Mathf.Clamp01(Time / Duration);
	}
}