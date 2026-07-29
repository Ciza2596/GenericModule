using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class SmallSpaceVE : VisualElement
	{
		// CONSTRUCTOR: --------------------------------------------------------------------- 
		
		[Preserve]
		public SmallSpaceVE() =>
			style.height = new StyleLength(10);
	}
}