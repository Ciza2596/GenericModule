using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class SpaceSmallVE : VisualElement
	{
		[Preserve]
		public SpaceSmallVE() =>
			style.height = new StyleLength(10);
	}
}