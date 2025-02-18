using System;
using UnityEngine.Scripting;
using UnityEngine.UIElements;

namespace CizaLocaleModule.Editor
{
    public abstract class BItemVE : VisualElement
    {
        // VARIABLE: -----------------------------------------------------------------------------

        protected abstract string[] DropAboveClasses { get; }
        protected abstract string[] DropBelowClasses { get; }

        [NonSerialized]
        protected readonly VisualElement _dropAbove = new VisualElement();

        [NonSerialized]
        protected readonly VisualElement _dropBelow = new VisualElement();

        // CONSTRUCTOR: --------------------------------------------------------------------- 

        [Preserve]
        protected BItemVE() { }

        // PUBLIC METHOD: ----------------------------------------------------------------------
        public virtual void Initialize()
        {
            foreach (var c in DropAboveClasses)
                _dropAbove.AddToClassList(c);
            foreach (var c in DropBelowClasses)
                _dropBelow.AddToClassList(c);
        }

        public virtual void Refresh() { }

        public virtual void DisplayAsNormal()
        {
            style.opacity = 1f;

            _dropAbove.style.display = DisplayStyle.None;
            _dropBelow.style.display = DisplayStyle.None;
        }

        public virtual void DisplayAsDrag() =>
            style.opacity = 0.25f;


        public virtual void DisplayAsTargetAbove() =>
            _dropAbove.style.display = DisplayStyle.Flex;


        public virtual void DisplayAsTargetBelow() =>
            _dropBelow.style.display = DisplayStyle.Flex;
        
    }
}