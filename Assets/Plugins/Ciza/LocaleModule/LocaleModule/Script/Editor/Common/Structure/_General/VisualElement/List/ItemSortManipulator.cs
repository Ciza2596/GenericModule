using UnityEngine.Scripting;

namespace CizaLocaleModule.Editor
{
    public class ItemSortManipulator : BSortManipulator<ItemVE>
    {
        [Preserve]
        public ItemSortManipulator(IListVE list) : base(list, true) { }
    }
}
