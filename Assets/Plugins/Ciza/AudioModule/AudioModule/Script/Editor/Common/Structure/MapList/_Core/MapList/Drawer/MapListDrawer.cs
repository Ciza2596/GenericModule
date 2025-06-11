using UnityEditor;

namespace CizaAudioModule.Editor.MapListVisual
{
	[CustomPropertyDrawer(typeof(MapList<>), true)]
	public class MapListDrawer : BMapListDrawer
	{
		protected virtual string KeyLabel => "Key";
		protected virtual string ValueLabel => "Value";

		protected override BMapListVE CreateListVE()
		{
			var listVE = new MapListVE(KeyLabel, ValueLabel, Property);
			listVE.Initialize();
			return listVE;
		}
	}
}