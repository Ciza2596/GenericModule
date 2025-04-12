using UnityEngine.UIElements;

namespace CizaLocaleModule.Editor
{
	public static class VisualElementUtils
	{
		public static void SetIsVisible(this VisualElement visualElement, bool isVisible) =>
			visualElement.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
	}
}