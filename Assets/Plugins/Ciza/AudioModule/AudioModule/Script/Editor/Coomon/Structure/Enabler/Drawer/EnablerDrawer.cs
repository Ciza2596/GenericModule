using UnityEditor;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	[CustomPropertyDrawer(typeof(BEnabler<,>), true)]
	public class EnablerDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			var enablerVE = new EnablerVE(property);
			enablerVE.Initialize();
			return enablerVE;
		}
	}
}