using UnityEngine;

namespace CizaLogModule.Implement
{
    public class UnityLogPrinter : ILogPrinter
    {
        public void PrintDebug(string message) =>
            Debug.Log("[Log::Debug] " + message);

        public void PrintInfo(string message) =>
            Debug.Log("[Log::Info] " + message);

        public void PrintWarn(string message) =>
            Debug.LogWarning("[Log::Warn] " + message);

        public void PrintError(string message) =>
            Debug.LogError("[Log::Error] " + message);
    }
}