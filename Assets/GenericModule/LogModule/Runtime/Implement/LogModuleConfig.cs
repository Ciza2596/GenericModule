using UnityEngine;

namespace CizaLogModule.Implement
{
    [CreateAssetMenu(fileName = "LogModuleConfig", menuName = "Ciza/LogModule/LogModuleConfig")]
    public class LogModuleConfig : ScriptableObject, ILogModuleConfig
    {
        [SerializeField]
        private bool _isPrintDebug;
        [SerializeField]
        private bool _isPrintInfo;
        [SerializeField]
        private bool _isPrintWarn;
        [SerializeField]
        private bool _isPrintError;
        
        

        public bool IsPrintDebug => _isPrintDebug;
        public bool IsPrintInfo => _isPrintInfo;
        public bool IsPrintWarn => _isPrintWarn;
        public bool IsPrintError => _isPrintError;
    }
}