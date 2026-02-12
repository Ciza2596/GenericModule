using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaInputModule
{
    [Serializable]
    public class VirtualMouseContainerConfigEnabler : BEnabler<VirtualMouseContainerConfig> {
        // VARIABLE: -----------------------------------------------------------------------------

        [SerializeField]
        protected VirtualMouseContainerConfig _value = new VirtualMouseContainerConfig();

        protected override VirtualMouseContainerConfig ValueImp
        {
            get => _value;
            set => _value = value;
        }

        // CONSTRUCTOR: ------------------------------------------------------------------------

        [Preserve]
        public VirtualMouseContainerConfigEnabler() { }

        [Preserve]
        public VirtualMouseContainerConfigEnabler(bool isEnable) : base(isEnable) { }

        [Preserve]
        public VirtualMouseContainerConfigEnabler(VirtualMouseContainerConfig value) : base(value) { }

        [Preserve]
        public VirtualMouseContainerConfigEnabler(bool isEnable, VirtualMouseContainerConfig value) : base(isEnable, value) { }
    }
}
