using UnityEditor;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	[CustomPropertyDrawer(typeof(IZomeraphyPanel), true)]
	public class ZomeraphyPanelDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new BBoxVE.ContentVE() { style = { paddingRight = 5 } };
			SerializationUtils.CreateChildProperties(container, property, SerializationUtils.ChildrenMode.ShowLabelsInChildren, 0);

			var box = new BoxVE(property);
			container.style.paddingRight = 0;
			box.Initialize(property.displayName, container);
			return box;
		}
	}
}