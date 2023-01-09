using UnityEngine.Assertions;

namespace LogModule
{
    public class LogModule
    {
        //private variable
        private ILogConfig _logConfig;
        private ILogPrinter _logPrinter;

        //module callback
        public LogModule(ILogConfig logConfig, ILogPrinter logPrinter)
        {
            Assert.IsNotNull(logConfig, "[LogModule::LogModule] LogConfig is null.");
            Assert.IsNotNull(logPrinter, "[LogModule::LogModule] LogPrinter is null.");
            
            _logConfig = logConfig;
            _logPrinter = logPrinter;
        }

        ~LogModule()
        {
            _logConfig = null;
            _logPrinter = null;
        }


        //public method
        public void Debug(string message)
        {
            if(_logConfig.IsPrintDebug)
                _logPrinter.PrintDebug(message);
        }

        public void Info(string message)
        {
            if (_logConfig.IsPrintInfo)
                _logPrinter.PrintInfo(message);
        }

        public void Warn(string message)
        {
            if(_logConfig.IsPrintWarn)
                _logPrinter.PrintWarn(message);
        }

        public void Error(string message)
        {
            if(_logConfig.IsPrintError)
                _logPrinter.PrintError(message);
        }
    }
}