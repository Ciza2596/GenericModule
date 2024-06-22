using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

namespace CizaInputModule
{
    public class RebindActionUIUtils
    {
        #region All

        public static string[] GetAllExcludingPaths()
        {
            var allPaths = new HashSet<string>();

            // Keyboard
            allPaths.AddRange(GetAllExcludingPathsWithoutMouse());

            // Mouse
            allPaths.AddRange(GetMouseAllPaths());

            return allPaths.ToArray();
        }

        public static string[] GetAllExcludingPathsWithoutMouse()
        {
            var allPaths = new HashSet<string>();

            // Keyboard
            allPaths.AddRange(GetKeyboardWASDPaths());
            allPaths.AddRange(GetKeyboardArrowPaths());
            allPaths.AddRange(GetKeyboardGenericFunctionPaths());

            // Xbox
            allPaths.AddRange(GetXboxLeftStickPaths());
            allPaths.AddRange(GetXboxDPadPaths());
            allPaths.AddRange(GetXboxRightStickPaths());
            allPaths.AddRange(GetXboxGenericFunctionPaths());

            // PS5
            allPaths.AddRange(GetPSLeftStickPaths());
            allPaths.AddRange(GetPSDPadPaths());
            allPaths.AddRange(GetPSRightStickPaths());
            allPaths.AddRange(GetPSGenericFunctionPaths());

            // Switch
            allPaths.AddRange(GetSwitchLeftStickPaths());
            allPaths.AddRange(GetSwitchDPadPaths());
            allPaths.AddRange(GetSwitchRightStickPaths());
            allPaths.AddRange(GetSwitchGenericFunctionPaths());

            // GamePad
            allPaths.AddRange(GetGamePadLeftStickPaths());
            allPaths.AddRange(GetGamePadDPadPaths());
            allPaths.AddRange(GetGamePadRightStickPaths());
            allPaths.AddRange(GetGamePadGenericFunctionPaths());

            return allPaths.ToArray();
        }

        #endregion

        #region Keyboard & Mouse

        // wasd
        public static string[] GetKeyboardWASDPaths() =>
            new[] { "<Keyboard>/w", "<Keyboard>/a", "<Keyboard>/s", "<Keyboard>/d" };


        public static string[] GetKeyboardArrowPaths() =>
            new[] { "<Keyboard>/upArrow", "<Keyboard>/leftArrow", "<Keyboard>/downArrow", "<Keyboard>/rightArrow" };

        // enter, backspace, esc
        public static string[] GetKeyboardGenericFunctionPaths() =>
            new[]
            {
                "<Keyboard>/escape", "<Keyboard>/backquote", "<Keyboard>/tab", "<Keyboard>/capsLock", "<Keyboard>/shift", "<Keyboard>/leftShift", "<Keyboard>/leftMeta", "<Keyboard>/rightMeta", "<Keyboard>/rightShift", "<Keyboard>/enter", "<Keyboard>/backslash", "<Keyboard>/backspace",
                "<Keyboard>/f1", "<Keyboard>/f2", "<Keyboard>/f3", "<Keyboard>/f4", "<Keyboard>/f5", "<Keyboard>/f6", "<Keyboard>/f7", "<Keyboard>/f8", "<Keyboard>/f9", "<Keyboard>/f10", "<Keyboard>/f11", "<Keyboard>/f12",
                "<Keyboard>/printScreen", "<Keyboard>/scrollLock", "<Keyboard>/pause",
                "<Keyboard>/home", "<Keyboard>/anyKey", "<Keyboard>/pageUp", "<Keyboard>/pageDown", "<Keyboard>/delete", "<Keyboard>/end", "<Keyboard>/numpadEnter", "<Keyboard>/contextMenu", "<Keyboard>/insert"
            };

        public static string[] GetMouseAllPaths() =>
            new[] { "<Mouse>/press", "<Mouse>/press", "<Mouse>/press", "<Mouse>/press", "<Mouse>/press", "<Mouse>/rightButton" };

        #endregion

        #region Xbox

        public static string[] GetXboxLeftStickPaths() =>
            new[] { "<XInputController>/leftStickPress", "<XInputController>/leftStick/up", "<XInputController>/leftStick/left", "<XInputController>/leftStick/left", "<XInputController>/leftStick/left" };

        public static string[] GetXboxDPadPaths() =>
            new[] { "<XInputController>/dpad/up", "<XInputController>/dpad/left", "<XInputController>/dpad/down", "<XInputController>/dpad/right" };

        public static string[] GetXboxRightStickPaths() =>
            new[] { "<XInputController>/rightStickPress", "<XInputController>/rightStick/up", "<XInputController>/rightStick/left", "<XInputController>/rightStick/left", "<XInputController>/rightStick/right" };


        // right, select
        public static string[] GetXboxGenericFunctionPaths() =>
            new[] { "<XInputController>/rightStick/right", "<XInputController>/select" };

        #endregion

        #region PS

        public static string[] GetPSLeftStickPaths() =>
            new[] { "<DualShockGamepad>/leftStickPress", "<DualShockGamepad>/leftStick/up", "<DualShockGamepad>/leftStick/left", "<DualShockGamepad>/leftStick/down", "<DualShockGamepad>/leftStick/down" };

        public static string[] GetPSDPadPaths() =>
            new[] { "<DualShockGamepad>/dpad/up", "<DualShockGamepad>/dpad/left", "<DualShockGamepad>/dpad/down", "<DualShockGamepad>/dpad/right" };

        public static string[] GetPSRightStickPaths() =>
            new[] { "<DualShockGamepad>/rightStickPress", "<DualShockGamepad>/rightStick/up", "<DualShockGamepad>/rightStick/left", "<DualShockGamepad>/rightStick/down", "<DualShockGamepad>/rightStick/right" };


        // select, touchpadButton, start
        public static string[] GetPSGenericFunctionPaths() =>
            new[] { "<DualShockGamepad>/select", "<DualShockGamepad>/touchpadButton", "<DualShockGamepad>/start" };

        #endregion

        #region Switch

        public static string[] GetSwitchLeftStickPaths() =>
            new[] { "<SwitchProControllerHID>/leftStickPress", "<SwitchProControllerHID>/leftStick/up", "<SwitchProControllerHID>/leftStick/left", "<SwitchProControllerHID>/leftStick/down", "<SwitchProControllerHID>/leftStick/right" };

        // wasd
        public static string[] GetSwitchDPadPaths() =>
            new[] { "<SwitchProControllerHID>/dpad/up", "<SwitchProControllerHID>/dpad/left", "<SwitchProControllerHID>/dpad/down", "<SwitchProControllerHID>/dpad/right" };

        public static string[] GetSwitchRightStickPaths() =>
            new[] { "<SwitchProControllerHID>/rightStickPress", "<SwitchProControllerHID>/rightStick/up", "<SwitchProControllerHID>/rightStick/left", "<SwitchProControllerHID>/rightStick/down", "<SwitchProControllerHID>/rightStick/right" };

        // enter, esc
        public static string[] GetSwitchGenericFunctionPaths() =>
            new[] { "<SwitchProControllerHID>/start", "<SwitchProControllerHID>/select", "<SwitchProControllerHID>/home", "<SwitchProControllerHID>/capture" };

        #endregion

        #region GamePad

        public static string[] GetGamePadLeftStickPaths() =>
            new[] { "<Gamepad>/leftStickPress", "<Gamepad>/leftStick/up", "<Gamepad>/leftStick/left", "<Gamepad>/leftStick/down", "<Gamepad>/leftStick/right" };

        // wasd
        public static string[] GetGamePadDPadPaths() =>
            new[] { "<Gamepad>/dpad/up", "<Gamepad>/dpad/left", "<Gamepad>/dpad/down", "<Gamepad>/dpad/right" };

        public static string[] GetGamePadRightStickPaths() =>
            new[] { "<Gamepad>/rightStickPress", "<Gamepad>/rightStick/up", "<Gamepad>/rightStick/left", "<Gamepad>/rightStick/down", "<Gamepad>/rightStick/right" };

        // enter, esc
        public static string[] GetGamePadGenericFunctionPaths() =>
            new[] { "<Gamepad>/start", "<Gamepad>/select" };

        #endregion
    }
}