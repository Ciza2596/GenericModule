using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
    public class FlexibleSpaceVE : VisualElement
    {
        [Preserve]
        public FlexibleSpaceVE() =>
            style.flexGrow = 1;
    }
}