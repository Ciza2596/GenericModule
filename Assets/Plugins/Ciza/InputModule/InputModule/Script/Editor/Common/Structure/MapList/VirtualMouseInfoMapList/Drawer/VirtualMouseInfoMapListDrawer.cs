using CizaInputModule.Editor.MapListVisual;
using UnityEditor;

namespace CizaInputModule.Editor
{
    [CustomPropertyDrawer(typeof(VirtualMouseInfoMapList))]
    public class VirtualMouseInfoMapListDrawer : BMapListDrawer
    {
        protected override BMapListVE CreateMapListVE(SerializedProperty property, BoxVE root)
        {
            var listVE = new VirtualMouseInfoMapListVE(property, false);
            listVE.Initialize();
            return listVE;
        }
    }
    
}
