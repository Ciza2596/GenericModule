using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Scripting;

namespace CizaAudioModule.Editor.MapListVisual
{
	public class MapItemVE : BMapItemVE
	{
		// VARIABLE: -----------------------------------------------------------------------------

		protected readonly string _keyLabel;
		protected readonly string _valueLabel;

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		public MapItemVE(string keyLabel, string valueLabel, BMapListVE root, SerializedProperty itemProperty) : base(root, itemProperty)
		{
			_keyLabel = keyLabel;
			_valueLabel = valueLabel;
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected override void CreateBodyContent()
		{
			var keyField = new PropertyField(KeyProperty, _keyLabel);
			keyField.Bind(ItemProperty.serializedObject);
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
					valueField.Bind(ItemProperty.serializedObject);
					_body.Add(valueField);
				}
			}

			keyField.RegisterCallback<SerializedPropertyChangeEvent>(_ => { RefreshHeadTitle(); });
		}
	}
}