using UnityEngine;
using UnityEngine.Assertions;

namespace CizaOptionModule.Implement
{
    public abstract class OptionSup : MonoBehaviour, IOptionSup
    {
        protected Option Option { get; private set; }

        public bool IsInitialized { get; private set; }

        public virtual void Initialize(Option option)
        {
            IsInitialized = true;

            Option = option;
            Assert.IsNotNull(Option, $"[{GetType().Name}::Awake] Option should be found.");
        }

        public virtual void Release(Option option)
        {
            IsInitialized = false;
        }
    }
}