using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaInputModule.Editor
{
	public class SmallSpaceVE : VisualElement
	{
		// CONSTRUCTOR: --------------------------------------------------------------------- 
		
		[Preserve]
		public SmallSpaceVE() =>
			style.height = new StyleLength(10);
	}
}