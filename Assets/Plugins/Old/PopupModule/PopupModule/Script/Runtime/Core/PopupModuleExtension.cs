using UnityEngine;

namespace CizaPopupModule
{
    public static class PopupModuleExtension
    {
        public static void HorizontalMovement(this PopupModule popupModule, string key, Vector2 direction)
        {
            if (direction.x > 0)
                popupModule.MoveToNext(key);

            else if (direction.x < 0)
                popupModule.MoveToPrevious(key);
        }

        public static void VerticalMovement(this PopupModule popupModule, string key, Vector2 direction)
        {
            if (direction.y < 0)
                popupModule.MoveToNext(key);

            else if (direction.y > 0)
                popupModule.MoveToPrevious(key);
        }
    }
}