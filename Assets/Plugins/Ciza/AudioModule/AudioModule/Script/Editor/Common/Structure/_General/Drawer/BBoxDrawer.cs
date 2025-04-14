using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public abstract class BBoxDrawer : PropertyDrawer
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[field: NonSerialized]
		protected SerializedProperty Property { get; set; }

		[field: NonSerialized]
		protected BBoxVE Root { get; set; }

		protected virtual string GetName() => Property.displayName;

		// PUBLIC METHOD: ----------------------------------------------------------------------

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			Property = property;

			Root = new BoxVE(property, CreateHeadAdditional());
			Root.Initialize(GetName(), CreateBody());

			return Root;
		}

		// PROTECT METHOD: --------------------------------------------------------------------
		protected virtual VisualElement CreateHeadAdditional() => null;

		protected virtual BBoxVE.IContent CreateBody()
		{
			var content = new BBoxVE.ContentVE();
			SerializationUtils.CreateChildProperties(content, Property, SerializationUtils.ChildrenKinds.ShowLabelsInChildren);
			return content;
		}
	}
}