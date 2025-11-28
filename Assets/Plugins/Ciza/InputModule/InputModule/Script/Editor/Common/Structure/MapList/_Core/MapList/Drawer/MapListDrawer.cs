using UnityEditor;

namespace CizaInputModule.Editor.MapListVisual
{
	[CustomPropertyDrawer(typeof(MapList<>), true)]
	public class MapListDrawer : BMapListDrawer
	{
		protected virtual string KeyLabel => "Key";
		protected virtual string ValueLabel => "Value";

		protected override BMapListVE CreateMapListVE(SerializedProperty property, BoxVE root)
		{
			var listVE = new MapListVE(property, false, KeyLabel, ValueLabel);
			listVE.Initialize();
			return listVE;
		}
	}
}