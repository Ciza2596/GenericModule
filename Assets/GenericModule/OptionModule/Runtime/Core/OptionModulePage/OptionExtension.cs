using System;
using System.Collections.Generic;
using System.Linq;
using CizaCore;
using UnityEngine.Assertions;

namespace CizaOptionModule
{
    public static class OptionExtension
    {
        public static string[] ToKeys(this Option[] options)
        {
            if (options == null)
                return Array.Empty<string>();

            var keys = new HashSet<string>();
            foreach (var option in options)
                keys.Add(option.Key);
            return keys.ToArray();
        }

        public static void InitializeOptions(this Option[] options, OptionModule optionModule, int playerIndex, string[] optionKeys, IOptionInfo[] optionInfos, Action<int, string, bool> onConfirm, Action<int, string> onPointerEnter, string logName)
        {
            Assert.IsTrue(optionKeys.Length <= options.Length, $"[{logName}::InitializeOptions] optionsLength: {options.Length} should be equal optionKeysLength: {optionKeys.Length}.");
            for (var i = 0; i < optionKeys.Length; i++)
            {
                var optionKey = optionKeys[i];
                if (!optionKey.HasValue())
                    continue;

                var optionInfo = optionInfos.FirstOrDefault(m_optionInfo => m_optionInfo.Key == optionKey);
                Assert.IsNotNull(optionInfo, $"[{logName}::InitializeOption] Not find optionInfo by Key:{optionKey}.");

                var option = options[i];
                if (option is null)
                    continue;

                option.Initialize(optionModule, playerIndex, optionKey, optionInfo.IsEnable, optionInfo.IsUnlock, optionInfo.IsNew, onConfirm, onPointerEnter, optionInfo.Parameters);
            }
        }

        public static void InitializeOptions(this Option[] options, int playerIndex, string[] isReadOptionKeys, Action<int, string, bool> onConfirm, Action<int, string> onPointerEnter, string logName)
        {
            Assert.IsNotNull(options, $"[{logName}::InitializeOptions] options is null.");
            foreach (var option in options)
                option.Initialize(null, playerIndex, option.Key, option.IsEnable, option.IsEnable, !isReadOptionKeys.Contains(option.Key), onConfirm, onPointerEnter, Array.Empty<object>());
        }
    }
}