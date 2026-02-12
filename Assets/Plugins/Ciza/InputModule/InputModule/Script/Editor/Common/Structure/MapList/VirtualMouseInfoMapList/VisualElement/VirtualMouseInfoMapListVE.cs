using CizaInputModule.Editor.MapListVisual;
using UnityEditor;
using UnityEngine.Scripting;

namespace CizaInputModule.Editor
{
    public class VirtualMouseInfoMapListVE : BMapListVE
    {
        // CONSTRUCTOR: ------------------------------------------------------------------------

        [Preserve]
        public VirtualMouseInfoMapListVE(SerializedProperty listProperty, bool isAutoRefresh) : base(listProperty, isAutoRefresh) { }

        // PROTECT METHOD: --------------------------------------------------------------------

        protected override ItemVE CreateItemVE(SerializedProperty itemProperty)
        {
            var itemVE = new VirtualMouseInfoMapItemVE(this, itemProperty);
            itemVE.Initialize();
            return itemVE;
        }
    }
}
