using CizaCore;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CizaOptionModule
{
    public class RollingLogicEventHandler
    {
        private readonly OptionModule _optionModule;

        private readonly RollingLogic _rollingLogic = new RollingLogic();

        public bool IsRollingHorizontal { get; private set; }
        public bool IsRollingVertical { get; private set; }

        public RollingLogicEventHandler(OptionModule optionModule) : this(optionModule, true, true) { }

        public RollingLogicEventHandler(OptionModule optionModule, bool isRollingHorizontal, bool isRollingVertical)
        {
            _optionModule = optionModule;
            SetIsRollingHorizontal(isRollingHorizontal);
            SetIsRollingVertical(isRollingVertical);
        }


        public void Initialize()
        {
            _optionModule.OnTick += _rollingLogic.Tick;
            _optionModule.OnAddPlayer += _rollingLogic.AddPlayer;
            _optionModule.OnRemovePlayer += _rollingLogic.RemovePlayer;

            _rollingLogic.OnMovementAsync += OnOptionModuleMovementAsync;

            _rollingLogic.ResetPlayerCount(_optionModule.PlayerCount);
        }

        public void Release()
        {
            _optionModule.OnTick -= _rollingLogic.Tick;
            _optionModule.OnAddPlayer -= _rollingLogic.AddPlayer;
            _optionModule.OnRemovePlayer -= _rollingLogic.RemovePlayer;

            _rollingLogic.OnMovementAsync -= OnOptionModuleMovementAsync;
        }

        public void SetIsRollingHorizontal(bool isRollingHorizontal) =>
            IsRollingHorizontal = isRollingHorizontal;

        public void SetIsRollingVertical(bool isRollingVertical) =>
            IsRollingVertical = isRollingVertical;


        public void MovementStart(int playerIndex, Vector2 direction, float rollingIntervalTime = RollingLogic.RollingIntervalTime) =>
            _rollingLogic.TurnOn(playerIndex, direction, rollingIntervalTime);

        public void MovementCancel(int playerIndex) =>
            _rollingLogic.TurnOff(playerIndex);


        private UniTask OnOptionModuleMovementAsync(int playerIndex, bool isFirst, Vector2 direction)
        {
            if (isFirst)
                return _optionModule.MovementAsync(playerIndex, direction);

            if (IsRollingHorizontal && IsRollingVertical)
                return _optionModule.MovementAsync(playerIndex, direction);

            if (IsRollingHorizontal)
                return _optionModule.HorizontalMovementAsync(playerIndex, direction);


            if (IsRollingVertical)
                return _optionModule.VerticalMovementAsync(playerIndex, direction);

            return UniTask.CompletedTask;
        }
    }
}