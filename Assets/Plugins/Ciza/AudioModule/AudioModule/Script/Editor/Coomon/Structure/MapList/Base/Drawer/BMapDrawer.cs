using UnityEditor;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor.MapListVisual
{
    public abstract class BMapDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            SerializationUtils.CreateChildProperties(container, property, SerializationUtils.ChildrenMode.ShowLabelsInChildren);
            return container;
        }
    }
}