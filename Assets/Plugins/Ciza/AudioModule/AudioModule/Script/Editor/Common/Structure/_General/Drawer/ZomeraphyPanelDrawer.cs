using UnityEditor;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	[CustomPropertyDrawer(typeof(IZomeraphyPanel), true)]
	public class ZomeraphyPanelDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var container = new BBoxVE.PropertyContentVE(property) { style = { paddingRight = 5 } };
			var boxVE = new BoxVE(property);
			container.style.paddingRight = 0;
			boxVE.Initialize(property.displayName, container);
			return boxVE;
		}
	}
}