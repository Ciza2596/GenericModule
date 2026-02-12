using CizaInputModule.Editor.MapListVisual;
using UnityEditor;
using UnityEngine.Scripting;

namespace CizaInputModule.Editor
{
	public class VirtualMouseInfoMapItemVE : BMapItemVE
	{
		// VARIABLE: -----------------------------------------------------------------------------

		public override string Title => $"Player Index: {ValueProperty.GetValue<VirtualMouseInfo>().PlayerIndex}";

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		public VirtualMouseInfoMapItemVE(BMapListVE root, SerializedProperty itemProperty) : base(root, itemProperty) { }

		protected override void CreateBodyContent() =>
			SerializationUtils.CreateChildProperties(_body, ValueProperty, SerializationUtils.ChildrenKinds.ShowLabelsInChildren, 0, OnRefreshBody);
	}
}