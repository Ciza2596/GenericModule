using CizaCore;
using CizaUniTask;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaOptionModule
{
    public class OptionModuleWithRollingLogic : OptionModule
    {
        private readonly RollingLogic _rollingLogic = new RollingLogic();

        public bool IsRollingHorizontal { get; private set; } = true;
        public bool IsRollingVertical { get; private set; } = true;

        public bool IsImmediately { get; private set; } = true;

        [Preserve]
        public OptionModuleWithRollingLogic(IOptionModuleConfig optionModuleConfig) : base(optionModuleConfig)
        {
            OnTick += _rollingLogic.Tick;
            OnAddPlayer += _rollingLogic.AddPlayer;
            OnRemovePlayer += _rollingLogic.RemovePlayer;
            OnConfirm += OnOptionModuleConfirm;

            _rollingLogic.OnMovementAsync += OnOptionModuleMovementAsync;
        }

        public void SetIsRollingHorizontal(bool isRollingHorizontal) =>
            IsRollingHorizontal = isRollingHorizontal;

        public void SetIsRollingVertical(bool isRollingVertical) =>
            IsRollingVertical = isRollingVertical;

        public void MovementStart(int playerIndex, Vector2 direction, bool isImmediately = true, float rollingIntervalTime = RollingLogic.RollingIntervalTime)
        {
            SetIsImmediately(isImmediately);
            _rollingLogic.TurnOn(playerIndex, direction, rollingIntervalTime);
        }

        public void MovementCancel(int playerIndex) =>
            _rollingLogic.TurnOff(playerIndex);


        private UniTask OnOptionModuleMovementAsync(int playerIndex, bool isFirst, Vector2 direction)
        {
            if (isFirst)
                return this.MovementAsync(playerIndex, direction, IsImmediately);

            if (IsRollingHorizontal && IsRollingVertical)
                return this.MovementAsync(playerIndex, direction, IsImmediately);

            if (IsRollingHorizontal)
                return this.HorizontalMovementAsync(playerIndex, direction, IsImmediately);


            if (IsRollingVertical)
            {
                this.VerticalMovement(playerIndex, direction);
                return UniTask.CompletedTask;
            }

            return UniTask.CompletedTask;
        }

        private void OnOptionModuleConfirm(int playerIndex, string optionKey, bool isUnlock) =>
            MovementCancel(playerIndex);

        private void SetIsImmediately(bool isImmediately) =>
            IsImmediately = isImmediately;
    }
}