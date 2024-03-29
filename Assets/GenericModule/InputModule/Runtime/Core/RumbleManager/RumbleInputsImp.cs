using System;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
    public class RumbleInputsImp : IRumbleInputs
    {
        private readonly InputModule _inputModule;

        public event Action<PlayerInput> OnPlayerLeft;

        public int PlayerCount => _inputModule.PlayerCount;

        public RumbleInputsImp(InputModule inputModule)
        {
            _inputModule = inputModule;

            _inputModule.OnPlayerLeft += OnPlayerLeftImp;
        }

        public bool TryGetCurrentControlScheme(int index, out string currentControlScheme) =>
            _inputModule.TryGetCurrentControlScheme(index, out currentControlScheme);

        public void ResetHaptics(int index) =>
            _inputModule.ResetHaptics(index);

        public void SetMotorSpeeds(int index, float lowFrequency, float highFrequency) =>
            _inputModule.SetMotorSpeeds(index, lowFrequency, highFrequency);

        private void OnPlayerLeftImp(PlayerInput playerInput) =>
            OnPlayerLeft?.Invoke(playerInput);
    }
}