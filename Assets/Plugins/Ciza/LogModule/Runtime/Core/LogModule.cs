using UnityEngine.Assertions;

namespace CizaLogModule
{
	public class LogModule
	{
		//private variable
		private readonly ILogModuleConfig _logModuleConfig;
		private readonly ILogPrinter      _logPrinter;

		//module callback
		public LogModule(ILogModuleConfig logModuleConfig, ILogPrinter logPrinter)
		{
			Assert.IsNotNull(logModuleConfig, "[LogModule::LogModule] LogConfig is null.");
			Assert.IsNotNull(logPrinter, "[LogModule::LogModule] LogPrinter is null.");

			_logModuleConfig = logModuleConfig;
			_logPrinter      = logPrinter;
		}

		//public method
		public void Debug(string message)
		{
			if (_logModuleConfig.IsPrintDebug)
				_logPrinter.PrintDebug(message);
		}

		public void Info(string message)
		{
			if (_logModuleConfig.IsPrintInfo)
				_logPrinter.PrintInfo(message);
		}

		public void Warn(string message)
		{
			if (_logModuleConfig.IsPrintWarn)
				_logPrinter.PrintWarn(message);
		}

		public void Error(string message)
		{
			if (_logModuleConfig.IsPrintError)
				_logPrinter.PrintError(message);
		}
	}
}
