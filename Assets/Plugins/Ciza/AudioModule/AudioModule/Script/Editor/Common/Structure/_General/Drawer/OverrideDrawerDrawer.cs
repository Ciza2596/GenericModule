using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	[CustomPropertyDrawer(typeof(OverrideDrawerAttribute))]
	public class OverrideDrawerDrawer : PropertyDrawer
	{
		// VARIABLE: -----------------------------------------------------------------------------
		protected virtual string CountText => "Count: ";

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			if (!property.isArray)
				return base.CreatePropertyGUI(property);

			var root = new BoxVE(property);
			root.Initialize(property.displayName, CreateBody_ListVE(property, root), CreateHeadAdditional_ListCountLabel(property, root));
			return root;
		}

		// PROTECT METHOD: --------------------------------------------------------------------
		protected virtual VisualElement CreateHeadAdditional_ListCountLabel(SerializedProperty property, BoxVE root)
		{
			var label = new Label(CountText + property.arraySize) { style = { paddingRight = 8 } };
			label.TrackPropertyValue(property, property_ =>
			{
				label.text = CountText + property_.arraySize;
				root.Refresh();
			});
			return label;
		}

		protected virtual BBoxVE.IContent CreateBody_ListVE(SerializedProperty property, BoxVE root)
		{
			var listVE = new ListVE(property, false);
			listVE.Initialize();
			return listVE;
		}
	}
}