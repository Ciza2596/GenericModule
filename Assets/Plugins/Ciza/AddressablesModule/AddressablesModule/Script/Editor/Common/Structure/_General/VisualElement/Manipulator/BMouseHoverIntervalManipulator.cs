using System;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaAddressablesModule.Editor
{
    public abstract class BMouseHoverIntervalManipulator : BMouseHoverManipulator<VisualElement>
    {
        // VARIABLE: -----------------------------------------------------------------------------

        [field: NonSerialized]
        protected virtual VisualElement Root { get; }

        [NonSerialized]
        protected IVisualElementScheduledItem _scheduledItem;

        [NonSerialized]
        protected VisualElement _hoverTarget;

        protected virtual Action OnHover { get; }
        protected virtual Func<VisualElement, bool> OnFilter { get; }

        protected virtual long ExecuteDelayMs => 1000;
        protected virtual long ExecuteIntervalMs => 600;

        // CONSTRUCTOR: ---------------------------------------------------------------------

        [Preserve]
        public BMouseHoverIntervalManipulator(VisualElement root, Action onHover, Func<VisualElement, bool> onFilter) : base()
        {
            Root = root;
            OnHover = onHover;
            OnFilter = onFilter;
        }

        // PROTECT METHOD: --------------------------------------------------------------------

        protected override void RegisterCallbacksOnTarget()
        {
            base.RegisterCallbacksOnTarget();
            target.RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            StopHoverSchedule();
            target.UnregisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
            base.UnregisterCallbacksFromTarget();
        }

        protected override void OnMouseEnter(MouseEnterEvent mouseEnterEvent, VisualElement targetVE)
        {
            if (mouseEnterEvent.pressedButtons != 1 || !CheckCanExecute(targetVE))
                return;

            StopHoverSchedule();
            _hoverTarget = targetVE;
            _scheduledItem = Root.schedule.Execute(ExecuteHover).Every(ExecuteIntervalMs).StartingIn(ExecuteDelayMs);
        }

        protected override void OnMouseExit(MouseLeaveEvent mouseLeaveEvent, VisualElement targetVE)
        {
            StopHoverSchedule();
        }

        protected override void OnMouseCaptureOut(MouseCaptureOutEvent mouseOutEvent, VisualElement targetVE)
        {
            StopHoverSchedule();
        }

        protected virtual bool CheckCanExecute(VisualElement targetVE) => targetVE is { panel: not null, enabledInHierarchy: true } && OnFilter?.Invoke(targetVE) == true;

        protected virtual void ExecuteHover()
        {
            if (!IsHovering || !CheckCanExecute(_hoverTarget))
            {
                StopHoverSchedule();
                return;
            }

            OnHover?.Invoke();

            if (!IsHovering || !CheckCanExecute(_hoverTarget))
                StopHoverSchedule();
        }

        protected virtual void StopHoverSchedule()
        {
            _scheduledItem?.Pause();
            _scheduledItem = null;
            _hoverTarget = null;
        }

        private void OnDetachFromPanel(DetachFromPanelEvent detachFromPanelEvent) =>
            StopHoverSchedule();
    }
}


