using CizaCore.UI;
using UnityEngine;


namespace CizaOptionModule.Implement
{
    public class VerticalScrollRectOptionViewSup : OptionViewSup
    {
        [SerializeField]
        private OptionView _optionView;

        [Space]
        [SerializeField]
        private VerticalScrollView _verticalScrollView;

        public override void Initialize()
        {
            _optionView.OnTick += _verticalScrollView.Tick;
            _optionView.OnSetCurrentCoordinate += OnSetCurrentCoordinate;
        }

        public override void Release()
        {
            _optionView.OnTick -= _verticalScrollView.Tick;
            _optionView.OnSetCurrentCoordinate -= OnSetCurrentCoordinate;
        }

        private void OnSetCurrentCoordinate(int playerIndex, Vector2Int previousCoordinate, Option previousOption, Vector2Int currentCoordinate, Option currentOption, bool isImmediately) =>
            _verticalScrollView.SetIndex(currentCoordinate.y, isImmediately);
    }
}