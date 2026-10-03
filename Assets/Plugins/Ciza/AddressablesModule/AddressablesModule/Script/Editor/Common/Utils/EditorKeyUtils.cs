using UnityEngine;
using UnityEngine.UIElements;

namespace CizaAddressablesModule.Editor
{
	public static class EditorKeyUtils
	{
		// Windows-ctrl, OSX-command
		public static bool CheckIsCtrl(this EventModifiers modifier) =>
			(Application.platform == RuntimePlatform.WindowsEditor && (modifier & EventModifiers.Control) != 0) || (Application.platform == RuntimePlatform.OSXEditor && (modifier & EventModifiers.Command) != 0);

		// Windows-alt, OSX-option
		public static bool CheckIsAlt(this EventModifiers modifier) =>
			(Application.platform == RuntimePlatform.WindowsEditor && (modifier & EventModifiers.Alt) != 0) || (Application.platform == RuntimePlatform.OSXEditor && (modifier & EventModifiers.Alt) != 0);

		// Windows-shift, OSX-shift
		public static bool CheckIsShift(this EventModifiers modifier) =>
			(Application.platform == RuntimePlatform.WindowsEditor && (modifier & EventModifiers.Shift) != 0) || (Application.platform == RuntimePlatform.OSXEditor && (modifier & EventModifiers.Shift) != 0);


		public static bool CheckIsCopy(this KeyDownEvent @event) =>
			CheckIsCtrl(@event.modifiers) && @event.keyCode == KeyCode.C;

		public static bool CheckIsCut(this KeyDownEvent @event) =>
			CheckIsCtrl(@event.modifiers) && @event.keyCode == KeyCode.X;

		public static bool CheckIsPaste(this KeyDownEvent @event) =>
			CheckIsCtrl(@event.modifiers) && @event.keyCode == KeyCode.V;

		public static bool CheckIsDuplicate(this KeyDownEvent @event) =>
			CheckIsCtrl(@event.modifiers) && @event.keyCode == KeyCode.D;

		public static bool CheckIsEnter(this KeyDownEvent @event) =>
			@event.keyCode == KeyCode.Return || @event.keyCode == KeyCode.KeypadEnter;

		public static bool CheckIsDelete(this KeyDownEvent @event) =>
			@event.keyCode == KeyCode.Delete;

		public static bool CheckIsSoftDelete(this KeyDownEvent @event) =>
			@event.shiftKey && @event.keyCode == KeyCode.Delete;

		public static bool CheckIsAnyDelete(this KeyDownEvent @event) =>
			CheckIsDelete(@event) || CheckIsSoftDelete(@event);


		public static bool CheckIsSelectAll(this KeyDownEvent @event) =>
			CheckIsCtrl(@event.modifiers) && @event.keyCode == KeyCode.A;

		public static bool CheckIsFocus(this KeyDownEvent @event) =>
			@event.keyCode == KeyCode.F;

		public static bool CheckIsFind(this KeyDownEvent @event) =>
			CheckIsCtrl(@event.modifiers) && @event.keyCode == KeyCode.F;

		public static bool CheckIsSave(this KeyDownEvent @event) =>
			CheckIsCtrl(@event.modifiers) && @event.keyCode == KeyCode.S;
	}
}

