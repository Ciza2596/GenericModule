using UnityEngine.UIElements;

namespace CizaLocaleModule.Editor
{
    public interface IListVE
    {
        int GetItemIndexOf(VisualElement item);

        int ClosestItemIndex(float cursorY);

        void RefreshItemDragUI(int sourceIndex, int targetIndex);
        void MoveItems(int sourceIndex, int destinationIndex);
        
        void Refresh();
    }
}