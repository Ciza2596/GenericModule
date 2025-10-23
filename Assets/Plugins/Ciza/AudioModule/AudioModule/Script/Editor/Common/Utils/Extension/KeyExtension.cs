using UnityEngine;

namespace CizaAudioModule.Editor
{
	public static class KeyExtension
	{
		// Windows-ctrl, OSX-command
		public static bool CheckIsCtrl(this EventModifiers modifier) =>
			(Application.platform == RuntimePlatform.WindowsEditor && (modifier & EventModifiers.Control) != 0) || (Application.platform == RuntimePlatform.OSXEditor && (modifier & EventModifiers.Command) != 0);
		
		// Windows-shift, OSX-shift
		public static bool CheckIsShift(this EventModifiers modifier) =>
			(Application.platform == RuntimePlatform.WindowsEditor && (modifier & EventModifiers.Shift) != 0) || (Application.platform == RuntimePlatform.OSXEditor && (modifier & EventModifiers.Shift) != 0);
	}
}