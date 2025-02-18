using System;
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

		[field: NonSerialized]
		protected virtual SerializedProperty Property { get; private set; }

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			Property = property;

			if (!Property.isArray)
				return base.CreatePropertyGUI(property);

			var root = new BoxVE(property, CreateHeadAdditional_ListCountLabel());
			root.Initialize(Property.displayName, CreateBody_ListVE());

			return root;
		}

		// PROTECT METHOD: --------------------------------------------------------------------
		protected virtual VisualElement CreateHeadAdditional_ListCountLabel()
		{
			var label = new Label(CountText + Property.arraySize) { style = { paddingRight = 8 } };
			label.TrackPropertyValue(Property, property => label.text = CountText + property.arraySize);
			return label;
		}

		protected virtual BBoxVE.IContent CreateBody_ListVE()
		{
			var listVE = new ListVE(Property);
			listVE.Initialize();
			return listVE;
		}
	}
}