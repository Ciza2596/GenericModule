

namespace LogModule
{
    public interface ILogPrinter
    {
        public void PrintDebug(string message);
        public void PrintInfo(string message);
        public void PrintWarn(string message);
        public void PrintError(string message);
    }
}
