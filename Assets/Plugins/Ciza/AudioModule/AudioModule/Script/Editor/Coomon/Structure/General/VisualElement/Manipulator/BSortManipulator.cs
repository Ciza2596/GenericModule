using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
    public abstract class BSortManipulator<TItem> : MouseManipulator where TItem : BItemVE
    {
        // VARIABLE: -----------------------------------------------------------------------------

        private readonly IListVE _list;

        private bool _isDragging;

        private int _startIndex = -1;
        private int _currentIndex = -1;

        private VisualElement _target;


        // CONSTRUCTOR: --------------------------------------------------------------------- 

        public BSortManipulator(IListVE list)
        {
            _list = list;
            activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
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

        protected virtual void OnMouseDown(MouseDownEvent eventMouseDown)
        {
            if (_isDragging)
            {
                eventMouseDown.StopImmediatePropagation();
                return;
            }

            if (!CanStartManipulation(eventMouseDown)) return;

            _target = eventMouseDown.currentTarget as VisualElement;

            _startIndex = _list.GetItemIndexOf(_target.GetFirstAncestorOfType<TItem>());
            _currentIndex = _startIndex;

            _isDragging = true;
            _target.CaptureMouse();

            eventMouseDown.StopPropagation();
        }

        protected virtual void OnMouseMove(MouseMoveEvent eventMouseMove)
        {
            if (!_isDragging)
                return;

            _currentIndex = _list.ClosestItemIndex(eventMouseMove.mousePosition.y);
            _list.RefreshItemDragUI(_startIndex, _currentIndex);

            eventMouseMove.StopPropagation();
        }

        protected virtual void OnMouseUp(MouseUpEvent eventMouseUp)
        {
            if (!_isDragging)
                return;

            if (!CanStopManipulation(eventMouseUp))
                return;

            _list.RefreshItemDragUI(-1, -1);

            if (_startIndex != _currentIndex)
                _list.MoveItems(_startIndex, _currentIndex);

            else
                _list.Refresh();

            _isDragging = false;

            _target.ReleaseMouse();
            _target = null;

            eventMouseUp.StopPropagation();
        }
    }
}