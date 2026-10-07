using System;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
	public abstract class BSortManipulator<TItemVE> : MouseManipulator where TItemVE : BItemVE
	{
		// VARIABLE: -----------------------------------------------------------------------------

		[NonSerialized]
		protected readonly IListVE _list;

		[NonSerialized]
		protected int _startIndex = -1;

		[NonSerialized]
		protected int _currentIndex = -1;

		[NonSerialized]
		protected VisualElement _targetVE;

		[NonSerialized]
		protected VisualElement _dragEventTarget;

		// PUBLIC VARIABLE: ---------------------------------------------------------------------

		[field: NonSerialized]
		public virtual bool HasFilter { get; }

		[field: NonSerialized]
		public virtual bool IsStopPropagation { get; }

		[field: NonSerialized]
		public virtual bool IsDragging { get; protected set; }


		// CONSTRUCTOR: --------------------------------------------------------------------- 

		[Preserve]
		protected BSortManipulator(IListVE list, bool hasFilter, bool isStopPropagation)
		{
			_list = list;
			HasFilter = hasFilter;
			IsStopPropagation = isStopPropagation;
		}

		// PROTECT METHOD: --------------------------------------------------------------------

		protected override void RegisterCallbacksOnTarget()
		{
			target.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.TrickleDown);
			target.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
			target.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
		}

		protected override void UnregisterCallbacksFromTarget()
		{
			target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
			target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
			target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
		}

		// CALLBACKS: -----------------------------------------------------------------------------

		protected virtual void OnMouseDown(MouseDownEvent mouseDownEvent)
		{
			if (IsDragging)
			{
				mouseDownEvent.StopImmediatePropagation();
				return;
			}

			if (HasFilter && !CanStartManipulation(mouseDownEvent))
				return;

			IsDragging = true;

			_targetVE = mouseDownEvent.currentTarget as VisualElement;
			OnMouseDown(mouseDownEvent, _targetVE);

			_targetVE.CaptureMouse();
			if (IsStopPropagation)
				mouseDownEvent.StopPropagation();
		}

		protected virtual void OnMouseMove(MouseMoveEvent mouseMoveEvent)
		{
			if (!IsDragging)
				return;

			OnMouseMove(mouseMoveEvent, _targetVE);
			if (IsStopPropagation)
				mouseMoveEvent.StopPropagation();
		}

		protected virtual void OnMouseUp(MouseUpEvent mouseUpEvent)
		{
			if (!IsDragging || (HasFilter && !CanStopManipulation(mouseUpEvent)))
				return;

			IsDragging = false;
			OnMouseUp(mouseUpEvent, _targetVE);

			if (IsStopPropagation)
				mouseUpEvent.StopPropagation();

			var captureTarget = _dragEventTarget ?? _targetVE;
			UnregisterDragCallbacks();
			if (captureTarget?.HasMouseCapture() == true)
				captureTarget.ReleaseMouse();

			_targetVE = null;
		}

		public virtual void PreserveDragOnTargetRefresh()
		{
			if (!IsDragging || _targetVE == null || _dragEventTarget != null)
				return;

			var stableTarget = _list as VisualElement;
			if (stableTarget == null || stableTarget == _targetVE)
				return;

			RegisterDragCallbacks(stableTarget);
			_dragEventTarget.CaptureMouse();
		}

		protected virtual void OnMouseCaptureOut(MouseCaptureOutEvent mouseCaptureOutEvent)
		{
			if (mouseCaptureOutEvent.target == _dragEventTarget)
				CancelDragging();
		}

		protected virtual void OnDragEventTargetDetachFromPanel(DetachFromPanelEvent detachFromPanelEvent)
		{
			if (detachFromPanelEvent.target == _dragEventTarget)
				CancelDragging();
		}

		protected virtual void OnMouseDown(MouseDownEvent mouseDownEvent, VisualElement targetVE)
		{
			_startIndex = _list.GetItemIndexOf(targetVE.GetFirstAncestorOfType<TItemVE>());
			_currentIndex = _startIndex;
		}

		protected virtual void OnMouseMove(MouseMoveEvent mouseMoveEvent, VisualElement targetVE)
		{
			if (_list.TryGetClosestItemIndex(mouseMoveEvent.mousePosition.y, out _currentIndex))
				_list.RefreshItemDragUI(_startIndex, _currentIndex);
			else
				_list.RefreshItemDragUI(-1, -1);
		}

		protected virtual void OnMouseUp(MouseUpEvent mouseUpEvent, VisualElement targetVE)
		{
			_list.TryGetClosestItemIndex(mouseUpEvent.mousePosition.y, out _currentIndex);
			_list.RefreshItemDragUI(-1, -1);

			if (_startIndex < 0 || _currentIndex < 0)
				_list.Refresh();
			else if (_startIndex != _currentIndex)
				_list.MoveItem(_startIndex, (_currentIndex > _startIndex) ? _currentIndex - 1 : _currentIndex);

			else
				_list.Refresh();
		}

		protected virtual void RegisterDragCallbacks(VisualElement dragEventTarget)
		{
			UnregisterDragCallbacks();
			_dragEventTarget = dragEventTarget;
			_dragEventTarget.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
			_dragEventTarget.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
			_dragEventTarget.RegisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);
			_dragEventTarget.RegisterCallback<DetachFromPanelEvent>(OnDragEventTargetDetachFromPanel);
		}

		protected virtual void UnregisterDragCallbacks()
		{
			if (_dragEventTarget == null)
				return;

			_dragEventTarget.UnregisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.TrickleDown);
			_dragEventTarget.UnregisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.TrickleDown);
			_dragEventTarget.UnregisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);
			_dragEventTarget.UnregisterCallback<DetachFromPanelEvent>(OnDragEventTargetDetachFromPanel);
			_dragEventTarget = null;
		}

		protected virtual void CancelDragging()
		{
			if (!IsDragging)
				return;

			IsDragging = false;
			_list.RefreshItemDragUI(-1, -1);
			UnregisterDragCallbacks();
			_targetVE = null;
			_startIndex = -1;
			_currentIndex = -1;
		}
	}
}
