using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaLocaleModule.Editor
{
    public class FlexibleSpaceVE : VisualElement
    {
        [Preserve]
        public FlexibleSpaceVE() =>
            style.flexGrow = 1;
    }
}