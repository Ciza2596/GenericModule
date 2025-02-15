using UnityEngine;

namespace CizaAudioModule
{
	public static class ObjectUtils
	{
		public static void DestroyOrImmediate(Object obj)
		{
			if (Application.isPlaying)
				Object.Destroy(obj);
			else
				Object.DestroyImmediate(obj);
		}
	}
}