using UnityEngine;
using UnityEngine.UIElements;

namespace CizaLocaleModule.Editor
{
	public static class VisualElementUtils
	{
		public static void SetBorder(this VisualElement element, float thickness, Color color = default, SideKinds sideKinds = SideKinds.All)
		{
			if (sideKinds.HasFlag(SideKinds.Left))
			{
				element.style.borderLeftWidth = thickness;
				element.style.borderLeftColor = color;
			}
			if (sideKinds.HasFlag(SideKinds.Right))
			{
				element.style.borderRightWidth = thickness;
				element.style.borderRightColor = color;
			}
			if (sideKinds.HasFlag(SideKinds.Top))
			{
				element.style.borderTopWidth = thickness;
				element.style.borderTopColor = color;
			}
			if (sideKinds.HasFlag(SideKinds.Bottom))
			{
				element.style.borderBottomWidth = thickness;
				element.style.borderBottomColor = color;
			}
		}

		public static void SetMargin(this VisualElement element, float margin, SideKinds sideKinds = SideKinds.All)
		{
			if (sideKinds.HasFlag(SideKinds.Left)) element.style.marginLeft = margin;
			if (sideKinds.HasFlag(SideKinds.Right)) element.style.marginRight = margin;
			if (sideKinds.HasFlag(SideKinds.Top)) element.style.marginTop = margin;
			if (sideKinds.HasFlag(SideKinds.Bottom)) element.style.marginBottom = margin;
		}

		public static void SetMargin(this VisualElement element, float left, float right, float top, float bottom)
		{
			element.style.marginLeft = left;
			element.style.marginRight = right;
			element.style.marginTop = top;
			element.style.marginBottom = bottom;
		}

		public static void SetAnchoredPosition(this VisualElement element, AnchorKinds anchorKind, Vector2 anchoredPosition)
		{
			StyleLength left = anchorKind.HasFlag(AnchorKinds.Left) ? anchoredPosition.x : StyleKeyword.Auto;
			StyleLength right = anchorKind.HasFlag(AnchorKinds.Right) ? anchoredPosition.x : StyleKeyword.Auto;
			StyleLength top = anchorKind.HasFlag(AnchorKinds.Top) ? anchoredPosition.y : StyleKeyword.Auto;
			StyleLength bottom = anchorKind.HasFlag(AnchorKinds.Bottom) ? anchoredPosition.y : StyleKeyword.Auto;
			element.style.left = left;
			element.style.right = right;
			element.style.top = top;
			element.style.bottom = bottom;
		}
		public static void SetIsVisible(this VisualElement visualElement, bool isVisible) =>
			visualElement.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
	}
}