using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CizaAudioModule.Editor
{
    public class DragManipulator : Clickable
    {
        // EVENT: ---------------------------------------------------------------------------------

        public event Action OnDragStart;
        public event Action OnDragFinish;
        public event Action OnDragMove;

        // PUBLIC VARIABLE: ---------------------------------------------------------------------

        public UnityEngine.UIElements.IEventHandler Target { get; private set; }

        public bool IsDragging { get; private set; }
        public Vector2 StartPosition { get; private set; }
        public Vector2 FinishPosition { get; private set; }

        public Vector2 Difference => FinishPosition - StartPosition;

        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public DragManipulator() : base(null, 250, 30) { }

        // CALLBACKS: -----------------------------------------------------------------------------

        protected override void ProcessDownEvent(EventBase eventDown, Vector2 position, int id)
        {
            Target = eventDown.target;
            IsDragging = true;
            StartPosition = position;
            FinishPosition = position;

            base.ProcessDownEvent(eventDown, position, id);
            OnDragStart?.Invoke();
        }

        protected override void ProcessCancelEvent(EventBase eventCancel, int id)
        {
            IsDragging = false;
            base.ProcessCancelEvent(eventCancel, id);
            OnDragFinish?.Invoke();
        }

        protected override void ProcessUpEvent(EventBase eventUp, Vector2 position, int id)
        {
            IsDragging = false;
            FinishPosition = position;
            base.ProcessUpEvent(eventUp, position, id);
            OnDragFinish?.Invoke();
        }

        protected override void ProcessMoveEvent(EventBase eventMove, Vector2 position)
        {
            FinishPosition = position;
            base.ProcessMoveEvent(eventMove, position);
            OnDragMove?.Invoke();
        }
    }
}