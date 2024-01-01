using CizaCore;
using UnityEngine;

namespace CizaOptionModule
{
    public class RollingLogicEventHandler
    {
        private readonly OptionModule _optionModule;

        private readonly RollingLogic _rollingLogic = new RollingLogic();

        public RollingLogicEventHandler(OptionModule optionModule) =>
            _optionModule = optionModule;


        public void Initialize()
        {
            _optionModule.OnTick += _rollingLogic.Tick;
            _optionModule.OnAddPlayer += _rollingLogic.AddPlayer;
            _optionModule.OnRemovePlayer += _rollingLogic.RemovePlayer;
            _rollingLogic.OnMovement += OnMovement;

            _rollingLogic.ResetPlayerCount(_optionModule.PlayerCount);
        }

        public void Release()
        {
            _optionModule.OnTick -= _rollingLogic.Tick;
            _optionModule.OnAddPlayer -= _rollingLogic.AddPlayer;
            _optionModule.OnRemovePlayer -= _rollingLogic.RemovePlayer;
            _rollingLogic.OnMovement -= OnMovement;
        }

        public void MovementStart(int playerIndex, Vector2 direction, float rollingIntervalTime = RollingLogic.RollingIntervalTime) =>
            _rollingLogic.TurnOn(playerIndex, direction, rollingIntervalTime);

        public void MovementCancel(int playerIndex) =>
            _rollingLogic.TurnOff(playerIndex);

        private async void OnMovement(int playerIndex, Vector2 direction) =>
            await _optionModule.MovementAsync(playerIndex, direction);
    }
}