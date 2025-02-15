using CizaAudioModule.Editor.MapListVisual;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class AudioInfoMapItemVE : BMapItemVE
	{
		protected virtual string ValueDataIdPath => "_dataId";

		protected virtual SerializedProperty ValueDataIdProperty =>
			ValueProperty.FindPropertyRelative(ValueDataIdPath);

		public override string Key
		{
			get
			{
				var dataId = ValueDataIdProperty.GetValue<string>();
				return dataId.CheckHasValue() ? dataId : "None";
			}
		}

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		public AudioInfoMapItemVE(BMapListVE root, SerializedProperty mapProperty) : base(root, mapProperty) { }

		protected override void DerivedInitialize()
		{
			SerializationUtils.CreateChildProperties(_body, ValueProperty, SerializationUtils.ChildrenMode.ShowLabelsInChildren, 0);
			var dataIdField = _body.Q<PropertyField>(ValueDataIdProperty.propertyPath);
			dataIdField.RegisterCallback<SerializedPropertyChangeEvent>(_ => { RefreshHeadTitle(); });
		}
	}
}