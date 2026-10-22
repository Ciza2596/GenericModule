using UnityEngine;

namespace CizaAudioModule
{
	public static class MathUtils
	{
		const float MIN_LINEAR_VOLUME = 0.001f;

		public static float GetLinearToLogarithmicScale(float value) =>
			Mathf.Log(Mathf.Clamp(value, MIN_LINEAR_VOLUME, 1f)) * 20.0f;

		public static float GetLogarithmicToLinearScale(float value)
		{
			var linear = Mathf.Exp(value / 20.0f);
			return linear <= MIN_LINEAR_VOLUME || Mathf.Approximately(linear, MIN_LINEAR_VOLUME) ? 0f : linear;
		}
	}
}