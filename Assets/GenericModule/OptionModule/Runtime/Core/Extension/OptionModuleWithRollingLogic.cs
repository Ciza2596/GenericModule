using CizaCore;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace CizaOptionModule
{
    public class OptionModuleWithRollingLogic: OptionModule
    {
        private readonly RollingLogic _rollingLogic = new RollingLogic();
        
        public bool IsRollingHorizontal { get; private set; }
        public bool IsRollingVertical { get; private set; }

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


        public void MovementStart(int playerIndex, Vector2 direction, float rollingIntervalTime = RollingLogic.RollingIntervalTime) =>
            _rollingLogic.TurnOn(playerIndex, direction, rollingIntervalTime);

        public void MovementCancel(int playerIndex) =>
            _rollingLogic.TurnOff(playerIndex);


        private UniTask OnOptionModuleMovementAsync(int playerIndex, bool isFirst, Vector2 direction)
        {
            if (isFirst)
                return this.MovementAsync(playerIndex, direction);

            if (IsRollingHorizontal && IsRollingVertical)
                return this.MovementAsync(playerIndex, direction);

            if (IsRollingHorizontal)
                return this.HorizontalMovementAsync(playerIndex, direction);


            if (IsRollingVertical)
            {
                this.VerticalMovement(playerIndex, direction);
                return UniTask.CompletedTask;
            }

            return UniTask.CompletedTask;
        }

        private void OnOptionModuleConfirm(int playerIndex, string optionKey, bool isUnlock) =>
            MovementCancel(playerIndex);
    }
}