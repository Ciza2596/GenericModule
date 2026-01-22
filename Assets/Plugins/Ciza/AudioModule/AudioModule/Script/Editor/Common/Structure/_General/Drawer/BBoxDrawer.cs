using UnityEditor;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public abstract class BBoxDrawer : PropertyDrawer
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected virtual string GetName(SerializedProperty property, VisualElement root) => property.displayName;

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var root = new BoxVE(property);
			root.Initialize(GetName(property, root), CreateBody(property, root), CreateHeadAdditional(property, root));
			return root;
		}

		// PROTECT METHOD: --------------------------------------------------------------------
		protected virtual VisualElement CreateHeadAdditional(SerializedProperty property, BoxVE root) => null;

		protected virtual BBoxVE.IContent CreateBody(SerializedProperty property, BoxVE root)
		{
			var content = new BBoxVE.ContentVE();
			SerializationUtils.CreateChildProperties(content, property, SerializationUtils.ChildrenKinds.ShowLabelsInChildren);
			return content;
		}
	}
}