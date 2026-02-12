using System.Linq;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
    public static class InputSystemUtils
    {
        public static bool TryGetHardwareMouse(out Mouse mouse)
        {
            mouse = InputSystem.devices.FirstOrDefault(device => device is Mouse { native: true }) as Mouse;
            return mouse != null;
        }
		
        public static bool TryGetMouse(string mouseName, out Mouse mouse)
        {
            mouse = InputSystem.GetDevice<Mouse>(mouseName);
            return mouse != null;
        }

        public static bool CheckMouseHasAnyInput(Mouse mouse)
        {
            if (mouse is not { added: true })
                return false;

            if (mouse.delta.ReadValue().sqrMagnitude > 0.01f * 0.01f)
                return true;

            if (mouse.scroll.ReadValue().sqrMagnitude > 0f)
                return true;

            return mouse.leftButton.isPressed ||
                   mouse.rightButton.isPressed ||
                   mouse.middleButton.isPressed ||
                   mouse.forwardButton.isPressed ||
                   mouse.backButton.isPressed;
        }
    }
}
