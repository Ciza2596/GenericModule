using System.Collections.Generic;
using System.Linq;
using CizaTimerModule;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
    public class RumbleManager
    {
        private readonly IRumbleManagerConfig _rumbleManagerConfig;
        private readonly IRumbleInputs _rumbleInputs;

        private readonly Dictionary<int, RumbleData> _rumbleDataMapByPlayerIndex = new Dictionary<int, RumbleData>();
        private readonly TimerModule _timerModule = new TimerModule();

        public string[] AllDataIds => _rumbleManagerConfig.AllDataIds;

        public bool CheckIsRumble(int playerIndex) =>
            _rumbleDataMapByPlayerIndex.ContainsKey(playerIndex);

        public bool TryGetOrder(int playerIndex, out int order)
        {
            if (!_rumbleDataMapByPlayerIndex.TryGetValue(playerIndex, out var rumbleData))
            {
                order = 0;
                return false;
            }

            order = rumbleData.Order;
            return true;
        }


        public RumbleManager(IRumbleManagerConfig rumbleManagerConfig, IRumbleInputs rumbleInputs)
        {
            _rumbleManagerConfig = rumbleManagerConfig;
            _rumbleInputs = rumbleInputs;

            _timerModule.Initialize();

            _rumbleInputs.OnPlayerLeft += OnPlayerLeftImp;
        }

        public void Tick(float deltaTime) =>
            _timerModule.Tick(deltaTime);

        public void Rumble(int playerIndex, string dataId)
        {
            if (!CheckHasPlayer(playerIndex) || !_rumbleManagerConfig.TryGetRumbleInfo(dataId, out var rumbleInfo) || !_rumbleInputs.TryGetCurrentControlScheme(playerIndex, out var currentControlScheme) || !rumbleInfo.TryGetControlSchemeInfo(currentControlScheme, out var controlSchemeInfo))
                return;

            if (TryGetOrder(playerIndex, out var order) && order > rumbleInfo.Order)
                return;

            _rumbleInputs.SetMotorSpeeds(playerIndex, controlSchemeInfo.LowFrequency, controlSchemeInfo.HighFrequency);
            AddRumbleData(playerIndex, rumbleInfo.Order, rumbleInfo.Duration);
        }

        public void RumbleAll(string dataId)
        {
            for (var i = 0; i < _rumbleInputs.PlayerCount; i++)
                Rumble(i, dataId);
        }

        public void Stop(int playerIndex)
        {
            if (!CheckIsRumble(playerIndex))
                return;

            RemoveTimerIdAndResetHaptics(playerIndex);
        }

        public void StopAll()
        {
            foreach (var index in _rumbleDataMapByPlayerIndex.Keys.ToArray())
                Stop(index);
        }

        private bool CheckHasPlayer(int playerIndex) =>
            playerIndex >= 0 && playerIndex < _rumbleInputs.PlayerCount;

        private void AddRumbleData(int playerIndex, int order, float duration)
        {
            RemoveTimer(playerIndex);

            var timerId = _timerModule.AddOnceTimer(duration, timerReadModel => { RemoveTimerIdAndResetHaptics(playerIndex); });
            _rumbleDataMapByPlayerIndex.Add(playerIndex, new RumbleData(order, timerId));
        }

        private void RemoveTimer(int playerIndex)
        {
            if (!_rumbleDataMapByPlayerIndex.TryGetValue(playerIndex, out var rumbleData))
                return;

            _timerModule.RemoveTimer(rumbleData.TimerId);
            _rumbleDataMapByPlayerIndex.Remove(playerIndex);
        }

        private void RemoveTimerIdAndResetHaptics(int playerIndex)
        {
            _rumbleInputs.ResetHaptics(playerIndex);
            _rumbleDataMapByPlayerIndex.Remove(playerIndex);
        }

        private void OnPlayerLeftImp(PlayerInput playerInput) =>
            Stop(playerInput.playerIndex);


        private class RumbleData
        {
            public int Order { get; }
            public string TimerId { get; }

            public RumbleData(int order, string timerId)
            {
                Order = order;
                TimerId = timerId;
            }
        }
    }
}