using System;
using UnityEngine.InputSystem;

namespace CizaInputModule
{
    public interface IRumbleInputs
    {
        public static IRumbleInputs Create(InputModule inputModule) =>
            new RumbleInputsImp(inputModule);

        event Action<PlayerInput> OnPlayerLeft;

        int PlayerCount { get; }

        bool TryGetCurrentControlScheme(int index, out string currentControlScheme);
        
        void ResetHaptics(int index);

        void SetMotorSpeeds(int index, float lowFrequency, float highFrequency);
    }
}