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
        public void Log(LogLevels logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevels.None:
                default:
                    break;
                case LogLevels.Debug:
                    if (_logConfig.IsPrintDebug)
                        _logPrinter.PrintDebug(message);
                    break;
                case LogLevels.Info:
                    if (_logConfig.IsPrintInfo)
                        _logPrinter.PrintInfo(message);
                    break;
                case LogLevels.Warn:
                    if (_logConfig.IsPrintWarn)
                        _logPrinter.PrintWarn(message);
                    break;
                case LogLevels.Error:
                    if (_logConfig.IsPrintError)
                        _logPrinter.PrintError(message);
                    break;
            }
        }
    }
}