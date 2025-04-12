using System;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public class MouseDragManipulator : BMouseDragManipulator<VisualElement>
	{
		[Preserve]
		public MouseDragManipulator() : this(new ManipulatorActivationFilter[]
		{
			new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse,
			}
		}) { }

		[Preserve]
		public MouseDragManipulator(ManipulatorActivationFilter[] activators, bool isStopPropagation = true) : base(isStopPropagation)
		{
			foreach (var activator in activators)
				this.activators.Add(activator);
		}
	}

	public abstract class BMouseDragManipulator<TTargetVE> : MouseManipulator where TTargetVE : VisualElement
	{
		// EVENT: ---------------------------------------------------------------------------------

		public event Action OnDragEnter;
		public event Action OnDragExit;
		public event Action OnDragMove;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		[field: NonSerialized]
		public virtual bool IsStopPropagation { get; }

		[field: NonSerialized]
		public virtual bool IsDragging { get; protected set; }

		#region Local Position

		[field: NonSerialized]
		public virtual Vector2 LocalStartPosition { get; protected set; }

		[field: NonSerialized]
		public virtual Vector2 LocalMovePosition { get; protected set; }

		public virtual Vector2 LocalDifference => LocalMovePosition - LocalStartPosition;

		#endregion

		#region Position

		[field: NonSerialized]
		public virtual Vector2 StartPosition { get; protected set; }

		[field: NonSerialized]
		public virtual Vector2 MovePosition { get; protected set; }

		public virtual Vector2 Difference => MovePosition - StartPosition;

		#endregion

		[Preserve]
		protected BMouseDragManipulator(bool isStopPropagation)
		{
			IsStopPropagation = isStopPropagation;
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<MouseDownEvent>(OnMouseDown);
			target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
			target.RegisterCallback<MouseUpEvent>(OnMouseUp);
			target.RegisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
			target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
			target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
			target.UnregisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);
		}

		protected virtual void OnMouseDown(MouseDownEvent mouseDownEvent)
		{
			if (IsDragging)
			{
				if (IsStopPropagation)
					mouseDownEvent.StopImmediatePropagation();
				return;
			}

			if (target is not TTargetVE targetVE)
				return;
			if (!CanStartManipulation(mouseDownEvent))
				return;

			IsDragging = true;

			LocalStartPosition = mouseDownEvent.localMousePosition;
			LocalMovePosition = LocalStartPosition;

			StartPosition = mouseDownEvent.mousePosition;
			MovePosition = StartPosition;

			OnMouseDown(mouseDownEvent, targetVE);
			OnDragEnterImp();

			targetVE.CaptureMouse();
			if (IsStopPropagation)
				mouseDownEvent.StopPropagation();
		}

		protected virtual void OnMouseMove(MouseMoveEvent mouseMoveEvent)
		{
			if (!IsDragging || target is not TTargetVE targetVE) return;

			LocalMovePosition = mouseMoveEvent.localMousePosition;
			MovePosition = mouseMoveEvent.mousePosition;

			OnMouseMove(mouseMoveEvent, targetVE);
			OnDragMoveImp();

			if (IsStopPropagation)
				mouseMoveEvent.StopPropagation();
		}

		protected virtual void OnMouseUp(MouseUpEvent mouseUpEvent)
		{
			if (!IsDragging || target is not TTargetVE targetVE) return;
			if (!CanStopManipulation(mouseUpEvent)) return;

			IsDragging = false;

			LocalMovePosition = mouseUpEvent.localMousePosition;
			MovePosition = mouseUpEvent.mousePosition;

			OnMouseUp(mouseUpEvent, targetVE);
			OnDragExitImp();
			if (IsStopPropagation)
				mouseUpEvent.StopPropagation();
			targetVE.ReleaseMouse();
		}

		protected virtual void OnMouseCaptureOut(MouseCaptureOutEvent mouseOutEvent)
		{
			if (!IsDragging || target is not TTargetVE targetVE) return;

			IsDragging = false;

			OnMouseCaptureOut(mouseOutEvent, targetVE);
			OnDragExitImp();

			targetVE.ReleaseMouse();
		}


		protected virtual void OnMouseDown(MouseDownEvent mouseDownEvent, TTargetVE targetVE) { }

		protected virtual void OnMouseMove(MouseMoveEvent mouseMoveEvent, TTargetVE targetVE) { }

		protected virtual void OnMouseUp(MouseUpEvent mouseUpEvent, TTargetVE targetVE) { }

		protected virtual void OnMouseCaptureOut(MouseCaptureOutEvent mouseOutEvent, TTargetVE targetVE) { }


		protected virtual void OnDragEnterImp() =>
			OnDragEnter?.Invoke();

		protected virtual void OnDragExitImp() =>
			OnDragExit?.Invoke();

		protected virtual void OnDragMoveImp() =>
			OnDragMove?.Invoke();
	}
}