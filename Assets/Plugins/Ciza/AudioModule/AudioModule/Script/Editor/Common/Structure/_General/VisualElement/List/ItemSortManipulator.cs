using UnityEngine.Scripting;

namespace CizaAudioModule.Editor
{
    public class ItemSortManipulator : BSortManipulator<ItemVE>
    {
        [Preserve]
        public ItemSortManipulator(IListVE list) : base(list, false, true) { }
    }
}
