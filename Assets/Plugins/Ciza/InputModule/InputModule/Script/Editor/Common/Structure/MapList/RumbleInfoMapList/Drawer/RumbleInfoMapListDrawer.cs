using CizaInputModule.Editor.MapListVisual;
using UnityEditor;

namespace CizaInputModule.Editor
{
	[CustomPropertyDrawer(typeof(RumbleInfoMapList))]
	public class RumbleInfoMapListDrawer : MapListDrawer
	{
		protected override string KeyLabel => "DataId";
	}
}