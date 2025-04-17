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
			_targetVE.ReleaseMouse();
			_targetVE = null;
		}

		protected virtual void OnMouseDown(MouseDownEvent mouseDownEvent, VisualElement targetVE)
		{
			_startIndex = _list.GetItemIndexOf(targetVE.GetFirstAncestorOfType<TItemVE>());
			_currentIndex = _startIndex;
		}

		protected virtual void OnMouseMove(MouseMoveEvent mouseMoveEvent, VisualElement targetVE)
		{
			_currentIndex = _list.ClosestItemIndex(mouseMoveEvent.mousePosition.y);
			_list.RefreshItemDragUI(_startIndex, _currentIndex);
		}

		protected virtual void OnMouseUp(MouseUpEvent mouseUpEvent, VisualElement targetVE)
		{
			_list.RefreshItemDragUI(-1, -1);

			if (_startIndex != _currentIndex)
				_list.MoveItems(_startIndex, (_currentIndex > _startIndex) ? _currentIndex - 1 : _currentIndex);

			else
				_list.Refresh();
		}
	}
}