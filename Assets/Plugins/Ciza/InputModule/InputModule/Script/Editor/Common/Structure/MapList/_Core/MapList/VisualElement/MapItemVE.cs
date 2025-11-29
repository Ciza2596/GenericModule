using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Scripting;

namespace CizaInputModule.Editor.MapListVisual
{
	public class MapItemVE : BMapItemVE
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly string _keyLabel;
		protected readonly string _valueLabel;

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		public MapItemVE(BMapListVE root, SerializedProperty itemProperty, string keyLabel, string valueLabel) : base(root, itemProperty)
		{
			_keyLabel = keyLabel;
			_valueLabel = valueLabel;
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected override void CreateBodyContent()
		{
			var keyField = new PropertyField(KeyProperty, _keyLabel);
			keyField.BindProperty(KeyProperty);
			_body.Add(keyField);

			var valueType = SerializationUtils.GetType(ValueProperty, false);
			if (valueType != null)
			{
				if (TypeUtils.CheckIsClassWithoutStringOrUnityObjSubclass(valueType))
				{
					_body.Add(new SmallSpaceVE());
					SerializationUtils.CreateChildProperties(_body, ValueProperty, SerializationUtils.ChildrenKinds.ShowLabelsInChildren, 0f, onChangeValue: OnRefreshBody);
				}
				else
				{
					var valueField = new PropertyField(ValueProperty, _valueLabel);
					valueField.BindProperty(ValueProperty);
					_body.Add(valueField);
				}
			}

			keyField.RegisterCallback<SerializedPropertyChangeEvent>(_ => { RefreshHeadTitle(); });
		}
	}
}