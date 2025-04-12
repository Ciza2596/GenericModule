using CizaAudioModule.Editor.MapListVisual;
using UnityEditor;
using UnityEngine.Scripting;

namespace CizaAudioModule.Editor
{
	public class AudioInfoMapItemVE : BMapItemVE
	{
		protected virtual string ValueDataIdPath => "_dataId";

		protected virtual SerializedProperty ValueDataIdProperty =>
			ValueProperty.FindPropertyRelative(ValueDataIdPath);

		public override string Title => ValueDataIdProperty.GetValue<string>();

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		public AudioInfoMapItemVE(BMapListVE root, SerializedProperty itemProperty) : base(root, itemProperty) { }

		protected override void CreateBodyContent()
		{
			SerializationUtils.CreateChildProperties(_body, ValueProperty, SerializationUtils.ChildrenMode.ShowLabelsInChildren, 0, OnRefreshBody);
		}
	}
}