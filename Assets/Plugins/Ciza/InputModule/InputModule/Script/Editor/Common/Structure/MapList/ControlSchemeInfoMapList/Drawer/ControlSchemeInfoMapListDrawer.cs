using CizaInputModule.Editor.MapListVisual;
using UnityEditor;

namespace CizaInputModule.Editor
{
	[CustomPropertyDrawer(typeof(ControlSchemeInfoMapList))]
	public class ControlSchemeInfoMapListDrawer : MapListDrawer
	{
		protected override string KeyLabel => "DataId";
	}
}