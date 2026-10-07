using System;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaInputModule.Editor
{
	public class PageButtonHoverManipulator : BMouseHoverIntervalManipulator
	{
		// CONSTRUCTOR: ---------------------------------------------------------------------

		[Preserve]
		public PageButtonHoverManipulator(PageBarVE root, Action onButtonHover, Func<VisualElement, bool> onFilter) : base(root, onButtonHover, onFilter) { }
	}
}
