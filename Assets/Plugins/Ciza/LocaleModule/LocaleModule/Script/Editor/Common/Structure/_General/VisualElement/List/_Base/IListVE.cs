using UnityEngine.UIElements;

namespace CizaLocaleModule.Editor
{
    public interface IListVE
    {
        int GetItemIndexOf(VisualElement item);

        bool TryGetClosestItemIndex(float cursorY, out int itemIndex);

        void RefreshItemDragUI(int sourceIndex, int targetIndex);
        void MoveItem(int sourceIndex, int destinationIndex);
        
        void Refresh();
    }
}