using CizaCore;
using UnityEngine;


namespace CizaOptionModule.Implement
{
    public class OptionViewWithVerticalScrollRect : OptionView
    {
        [SerializeField]
        private VerticalScrollView _verticalScrollView;


        public override void Tick(float deltaTime) =>
            _verticalScrollView.Tick(deltaTime);


        public override void OnSetCurrentCoordinate(int playerIndex, Vector2Int previousCoordinate, Option previousOption, Vector2Int currentCoordinate, Option currentOption, bool isImmediately) =>
            _verticalScrollView.SetIndex(currentCoordinate.y, isImmediately);
    }
}