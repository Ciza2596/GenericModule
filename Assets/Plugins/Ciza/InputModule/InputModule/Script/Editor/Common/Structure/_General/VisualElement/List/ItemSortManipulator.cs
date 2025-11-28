using UnityEngine.Scripting;

namespace CizaInputModule.Editor
{
	public class ItemSortManipulator: BSortManipulator<ItemVE>
	{
		[Preserve]
		public ItemSortManipulator(IListVE list) : base(list, false, true) { }
	}
}