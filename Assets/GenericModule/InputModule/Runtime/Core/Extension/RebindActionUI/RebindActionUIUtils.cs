using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;

namespace CizaInputModule
{
    public class RebindActionUIUtils
    {
        #region All

        public static string[] AllPaths()
        {
            var allPaths = new HashSet<string>();

            // Keyboard
            allPaths.AddRange(AllPathsWithoutMouse());

            // Mouse
            allPaths.AddRange(MouseAllPaths());

            return allPaths.ToArray();
        }

        public static string[] AllPathsWithoutMouse()
        {
            var allPaths = new HashSet<string>();

            // Keyboard
            allPaths.AddRange(KeyboardWASDPaths());
            allPaths.AddRange(KeyboardArrowPaths());
            allPaths.AddRange(KeyboardGenericFunctionPaths());

            // Xbox
            allPaths.AddRange(XboxLeftStickPaths());
            allPaths.AddRange(XboxDPadPaths());
            allPaths.AddRange(XboxRightStickPaths());
            allPaths.AddRange(XboxGenericFunctionPaths());

            // PS5
            allPaths.AddRange(PSLeftStickPaths());
            allPaths.AddRange(PSDPadPaths());
            allPaths.AddRange(PSRightStickPaths());
            allPaths.AddRange(PSGenericFunctionPaths());

            // Switch
            allPaths.AddRange(SwitchLeftStickPaths());
            allPaths.AddRange(SwitchDPadPaths());
            allPaths.AddRange(SwitchRightStickPaths());
            allPaths.AddRange(SwitchGenericFunctionPaths());

            return allPaths.ToArray();
        }

        #endregion

        #region Keyboard & Mouse

        // wasd
        public static string[] KeyboardWASDPaths() =>
            new[] { "<Keyboard>/w", "<Keyboard>/a", "<Keyboard>/s", "<Keyboard>/d" };


        public static string[] KeyboardArrowPaths() =>
            new[] { "<Keyboard>/upArrow", "<Keyboard>/leftArrow", "<Keyboard>/downArrow", "<Keyboard>/rightArrow" };

        // enter, backspace, esc
        public static string[] KeyboardGenericFunctionPaths() =>
            new[]
            {
                "<Keyboard>/escape", "<Keyboard>/backquote", "<Keyboard>/tab", "<Keyboard>/capsLock", "<Keyboard>/shift", "<Keyboard>/leftShift", "<Keyboard>/leftMeta", "<Keyboard>/rightMeta", "<Keyboard>/rightShift", "<Keyboard>/enter", "<Keyboard>/backslash", "<Keyboard>/backspace",
                "<Keyboard>/f1", "<Keyboard>/f2", "<Keyboard>/f3", "<Keyboard>/f4", "<Keyboard>/f5", "<Keyboard>/f6", "<Keyboard>/f7", "<Keyboard>/f8", "<Keyboard>/f9", "<Keyboard>/f10", "<Keyboard>/f11", "<Keyboard>/f12",
                "<Keyboard>/printScreen", "<Keyboard>/scrollLock", "<Keyboard>/pause",
                "<Keyboard>/home", "<Keyboard>/anyKey", "<Keyboard>/pageUp", "<Keyboard>/pageDown", "<Keyboard>/delete", "<Keyboard>/end", "<Keyboard>/numpadEnter"
            };

        public static string[] MouseAllPaths() =>
            new[] { "<Mouse>/press", "<Mouse>/press", "<Mouse>/press", "<Mouse>/press", "<Mouse>/press", "<Mouse>/rightButton" };

        #endregion

        #region Xbox

        public static string[] XboxLeftStickPaths() =>
            new[] { "<XInputController>/leftStick/up", "<XInputController>/leftStick/left", "<XInputController>/leftStick/left", "<XInputController>/leftStick/left" };

        public static string[] XboxDPadPaths() =>
            new[] { "<XInputController>/dpad/up", "<XInputController>/dpad/left", "<XInputController>/dpad/down", "<XInputController>/dpad/right" };

        public static string[] XboxRightStickPaths() =>
            new[] { "<XInputController>/rightStick/up", "<XInputController>/rightStick/left", "<XInputController>/rightStick/left", "<XInputController>/rightStick/right" };


        // right, select
        public static string[] XboxGenericFunctionPaths() =>
            new[] { "<XInputController>/rightStick/right", "<XInputController>/select" };

        #endregion

        #region PS

        public static string[] PSLeftStickPaths() =>
            new[] { "<DualShockGamepad>/leftStick/up", "<DualShockGamepad>/leftStick/left", "<DualShockGamepad>/leftStick/down", "<DualShockGamepad>/leftStick/down" };

        public static string[] PSDPadPaths() =>
            new[] { "<DualShockGamepad>/dpad/up", "<DualShockGamepad>/dpad/left", "<DualShockGamepad>/dpad/down", "<DualShockGamepad>/dpad/right" };

        public static string[] PSRightStickPaths() =>
            new[] { "<DualShockGamepad>/rightStick/up", "<DualShockGamepad>/rightStick/left", "<DualShockGamepad>/rightStick/down", "<DualShockGamepad>/rightStick/right" };


        // select, touchpadButton, start
        public static string[] PSGenericFunctionPaths() =>
            new[] { "<DualShockGamepad>/select", "<DualShockGamepad>/touchpadButton", "<DualShockGamepad>/start" };

        #endregion

        #region Switch

        public static string[] SwitchLeftStickPaths() =>
            new[] { "<SwitchProControllerHID>/leftStick/up", "<SwitchProControllerHID>/leftStick/left", "<SwitchProControllerHID>/leftStick/down", "<SwitchProControllerHID>/leftStick/right" };

        // wasd
        public static string[] SwitchDPadPaths() =>
            new[] { "<SwitchProControllerHID>/dpad/up", "<SwitchProControllerHID>/dpad/left", "<SwitchProControllerHID>/dpad/down", "<SwitchProControllerHID>/dpad/right" };

        public static string[] SwitchRightStickPaths() =>
            new[] { "<SwitchProControllerHID>/rightStick/up", "<SwitchProControllerHID>/rightStick/left", "<SwitchProControllerHID>/rightStick/down", "<SwitchProControllerHID>/rightStick/right" };

        // enter, esc
        public static string[] SwitchGenericFunctionPaths() =>
            new[] { "<SwitchProControllerHID>/start", "<SwitchProControllerHID>/select", "<SwitchProControllerHID>/home", "<SwitchProControllerHID>/capture" };

        #endregion
    }
}