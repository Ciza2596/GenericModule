using UnityEngine.InputSystem;

namespace CizaInputModule
{
    public static class InputSystemExtension
    {
        public static bool CheckMouseHasAnyInput(this Mouse mouse) =>
            InputSystemUtils.CheckMouseHasAnyInput(mouse);
    }
}
