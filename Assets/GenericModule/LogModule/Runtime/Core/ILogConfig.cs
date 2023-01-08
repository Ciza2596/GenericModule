
namespace LogModule
{
    public interface ILogConfig
    {
        //Dev, Demo, Editor使用
        public bool IsPrintDebug { get; }
        
        //以下為正式版本使用
        public bool IsPrintInfo { get; }
        public bool IsPrintWarn { get; }
        public bool IsPrintError { get; }
    }
}