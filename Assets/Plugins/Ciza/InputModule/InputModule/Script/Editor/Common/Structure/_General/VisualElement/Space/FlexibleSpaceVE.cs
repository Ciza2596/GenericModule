using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaInputModule.Editor
{
    public class FlexibleSpaceVE : VisualElement
    {
        // CONSTRUCTOR: --------------------------------------------------------------------- 
        
        [Preserve]
        public FlexibleSpaceVE() =>
            style.flexGrow = 1;
    }
}