using System;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class MouseHoverManipulator : BMouseHoverManipulator<VisualElement>
	{
		// CONSTRUCTOR: --------------------------------------------------------------------- 
		
		[Preserve]
		public MouseHoverManipulator() : base() { }
	}

	public abstract class BMouseHoverManipulator<TTargetVE> : MouseManipulator where TTargetVE : VisualElement
	{
		// PUBLIC VARIABLE: ---------------------------------------------------------------------


		[field: NonSerialized]
		public virtual bool IsHovering { get; protected set; }

		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		protected BMouseHoverManipulator() { }

		// PROTECT METHOD: --------------------------------------------------------------------

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
			target.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
			target.RegisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<MouseEnterEvent>(OnMouseEnter);
			target.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
			target.UnregisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);
		}

		// EVENT CALLBACK: ---------------------------------------------------------------------

		protected virtual void OnMouseEnter(MouseEnterEvent mouseEnterEvent)
		{
			if (IsHovering || target is not TTargetVE targetVE) return;

			IsHovering = true;
			OnMouseEnter(mouseEnterEvent, targetVE);
		}

		protected virtual void OnMouseLeave(MouseLeaveEvent mouseLeaveEvent)
		{
			if (!IsHovering || target is not TTargetVE targetVE) return;

			IsHovering = false;
			OnMouseExit(mouseLeaveEvent, targetVE);
		}

		protected virtual void OnMouseCaptureOut(MouseCaptureOutEvent mouseOutEvent)
		{
			if (!IsHovering || target is not TTargetVE targetVE) return;

			IsHovering = false;
			OnMouseCaptureOut(mouseOutEvent, targetVE);
		}

		protected virtual void OnMouseEnter(MouseEnterEvent mouseEnterEvent, TTargetVE targetVE) { }

		protected virtual void OnMouseExit(MouseLeaveEvent mouseLeaveEvent, TTargetVE targetVE) { }

		protected virtual void OnMouseCaptureOut(MouseCaptureOutEvent mouseOutEvent, TTargetVE targetVE) { }
	}
}