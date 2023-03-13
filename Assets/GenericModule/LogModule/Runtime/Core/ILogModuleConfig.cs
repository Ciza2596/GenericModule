
namespace CizaLogModule
{
    public interface ILogModuleConfig
    {
        //Dev, Demo, Editor
        public bool IsPrintDebug { get; }
        
        //Production
        public bool IsPrintInfo { get; }
        public bool IsPrintWarn { get; }
        public bool IsPrintError { get; }
    }
}